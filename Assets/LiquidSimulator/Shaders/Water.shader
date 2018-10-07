Shader "Custom/Water"
{
	Properties
	{
		_Specular("Specular", float) = 0
		_Gloss("Gloss", float) = 0
		_Refract("Refract", float) = 0
		_Height ("Height(position, color)", vector) = (0.36, 0, 0, 0)
		_Fresnel ("Fresnel", float) = 3.0
		_BaseColor ("BaseColor", color) = (1,1,1,1)
		_WaterColor ("WaterColor", color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 100

		GrabPass {}

		Pass
		{
			zwrite off
			blend srcalpha oneminussrcalpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Utils.cginc"
			#include "Lighting.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 proj0 : TEXCOORD2;
				float4 proj1 : TEXCOORD3;
				float4 vertex : SV_POSITION;
				float4 TW0 : TEXCOORD4;
				float4 TW1 : TEXCOORD5;
				float4 TW2 : TEXCOORD6;
				L_SHADOWCOORDS(7, 8)
			};

			sampler2D _GrabTexture;
			
			sampler2D_float _CameraDepthTexture;

			half _Specular;
			half _Gloss;

			half4 _BaseColor;
			half4 _WaterColor;

			half _Refract;

			half _Fresnel;

			float2 _Height;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				float4 projPos = UnityObjectToClipPos(v.vertex);
				
				o.proj0 = ComputeGrabScreenPos(projPos);
				o.proj1 = ComputeScreenPos(projPos);

				float height = DecodeHeight(tex2Dlod(_LiquidHeightMap, float4(v.texcoord.xy,0,0)));
				v.vertex.y += height*_Height.x;
				o.uv = v.texcoord;
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);

				COMPUTE_EYEDEPTH(o.proj0.z);

				L_TRANSFER_SHADOWCOORDS(v, o);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldTan = UnityObjectToWorldDir(v.tangent.xyz);
				float tanSign = v.tangent.w * unity_WorldTransformParams.w;
				float3 worldBinormal = cross(worldNormal, worldTan)*tanSign;
				o.TW0 = float4(worldTan.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.TW1 = float4(worldTan.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.TW2 = float4(worldTan.z, worldBinormal.z, worldNormal.z, worldPos.z);
		
				return o;
			}

			half3 WaterColor(float3 refractColor, float3 reflectColor, float3 worldPos, float height, float3 worldNormal, float3 lightDir, float3 viewDir) {
				float f = pow(clamp(1.0 - dot(worldNormal, viewDir), 0.0, 1.0), _Fresnel) * 0.65;
				float3 viewDis = -UnityWorldSpaceViewDir(worldPos);

				float3 refraccol = _BaseColor.rgb*refractColor + pow(dot(worldNormal, lightDir) * 0.4 + 0.6, 80.0) * _WaterColor.rgb * 0.12;

				float3 color = lerp(refraccol, reflectColor, f);

				float atten = max(1.0 - dot(viewDis, viewDis) * 0.001, 0.0);
				color += _WaterColor.rgb*refractColor * (height*_Height.y) * 0.18 * atten;

				return color;
			}

			
			half4 frag (v2f i) : SV_Target
			{
				float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.proj1));
				float deltaDepth = depth - i.proj0.z;

				
				float3 normal = UnpackNormal(tex2D(_LiquidNormalMap, i.uv));

				L_SHADOW_ATTEN_REFRACT(atten, i, (normal.xy*_Refract));

				float height = DecodeHeight(tex2D(_LiquidHeightMap, i.uv));

				float3 worldNormal = float3(dot(i.TW0.xyz, normal), dot(i.TW1.xyz, normal), dot(i.TW2.xyz, normal));
				float3 worldPos = float3(i.TW0.w, i.TW1.w, i.TW2.w);

				float3 lightDir = normalize(GetLightDirection(worldPos.xyz));
				float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				float2 projUv = i.proj0.xy / i.proj0.w + normal.xy*_Refract;
				half4 col = tex2D(_GrabTexture, projUv);
				half4 reflcol = tex2D(_LiquidReflectMap, projUv);

				col.rgb = WaterColor(col.rgb, reflcol.rgb, worldPos, height, worldNormal, lightDir, viewDir);

				float3 hdir = normalize(lightDir + viewDir);

				float ndh = max(0, dot(worldNormal, hdir));

				col.rgb += internalWorldLightColor.rgb * pow(ndh, _Specular*128.0) * _Gloss*atten;

				UNITY_APPLY_FOG(i.fogCoord, col);

				col.a = 1.0;
				return col;
			}
			ENDCG
		}
	}
}
