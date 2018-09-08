Shader "Unlit/Water"
{
	Properties
	{
		_LiquidHeightMap ("HeightMap", 2D) = "" {}
		_LiquidReflectMap ("ReflectMap", 2D) = "" {}
		_Specular("Specular", float) = 0
		_Gloss("Gloss", float) = 0
		_Refract("Refract", float) = 0
		_Height ("Height", float) = 0
		_Range("Range", vector) = (0, 0, 0, 0)
		_ShallowColor("ShallowColor", color) = (1,1,1,1)
		_DeepColor("DeepColor", color) = (1,1,1,1)
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
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
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
			};

			sampler2D _GrabTexture;
			sampler2D _LiquidHeightMap;
			sampler2D _LiquidReflectMap;
			sampler2D_float _CameraDepthTexture;

			half _Specular;
			half _Gloss;

			half4 _ShallowColor;
			half4 _DeepColor;

			half4 _Range;
			half _Refract;

			float _Height;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				float4 projPos = UnityObjectToClipPos(v.vertex);
				
				o.proj0 = ComputeGrabScreenPos(projPos);
				o.proj1 = ComputeScreenPos(projPos);

				float height = tex2Dlod(_LiquidHeightMap, float4(v.texcoord.xy,0,0));
				v.vertex.y += (height-0.5)*_Height;
				o.uv = v.texcoord;
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);

				COMPUTE_EYEDEPTH(o.proj0.z);

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

			
			half4 frag (v2f i) : SV_Target
			{
				float depth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture, i.proj1));
				float deltaDepth = depth - i.proj0.z;

				half3 ranges = saturate(_Range.xyz * deltaDepth);
				ranges.y = 1.0 - ranges.y;
				ranges.y = lerp(ranges.y, ranges.y * ranges.y * ranges.y, 0.5);
				
				float4 heightMapCol = tex2D(_LiquidHeightMap, i.uv);
				float height;
				float3 normal;
				DecodeDepthNormal(heightMapCol, height, normal);

				float3 worldNormal = float3(dot(i.TW0.xyz, normal), dot(i.TW1.xyz, normal), dot(i.TW2.xyz, normal));
				float3 worldPos = float3(i.TW0.w, i.TW1.w, i.TW2.w);

				float3 lightDir = UnityWorldSpaceLightDir(worldPos);
				float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));

				float2 projUv = i.proj0.xy / i.proj0.w + normal.xy*_Refract;
				half4 col = tex2D(_GrabTexture, projUv);
				half4 reflcol = tex2D(_LiquidReflectMap, projUv);

				col.rgb = lerp(col.rgb, reflcol.rgb, 0.5);

				half3 watercol = lerp(_DeepColor.rgb, _ShallowColor.rgb, ranges.y);

				float3 hdir = normalize(lightDir + viewDir);

				float ndh = max(0, dot(worldNormal, hdir));

				col.rgb *= watercol.rgb;

				col.rgb += _LightColor0.rgb * pow(ndh, _Specular*128.0) * _Gloss;

				//col.rgb *= _Color.rgb;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				col.a = ranges.x;
				return float4(height,height,height,1);
				//return float4(normal, 1);
			}
			ENDCG
		}
	}
}
