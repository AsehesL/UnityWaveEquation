// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Lighting/Forward/BlinnPhongUnderWater"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Normal][NoScaleOffset]_BumpTex("BumpTex", 2D) = "bump" {}
		_SpecTex ("SpecTex", 2D) = "white" {}
		_AOTex ("AoTex", 2D) = "white" {}
		_SpecColor ("SpecCol", color) = (1,1,1,1)
		_Specular ("Specular", float) = 0
		_Gloss ("Gloss", float) = 0
		_GI ("GI", cube) = "" {}
		_GILod ("GILod", float) = 0
		_GIColor ("GIColor", color) = (0,0,0,1)
		_Range("Range", vector) = (0, 0, 0, 0)
		_ShallowColor("ShallowColor", color) = (1,1,1,1)
		_DeepColor("DeepColor", color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "Utils.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 aouv : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 vertex : SV_POSITION;
				L_SHADOWCOORDS(3, 4)
				float4 RT0 : TEXCOORD5;
				float4 RT1 : TEXCOORD6;
				float4 RT2 : TEXCOORD7;
			};

			sampler2D _BumpTex;
			sampler2D _MainTex;
			sampler2D _SpecTex;
			sampler2D _AOTex;
			samplerCUBE _GI;
			float4 _MainTex_ST;

			float _Specular;
			half _Gloss;

			half4 _GIColor;
			half _GILod;

			half4 _ShallowColor;
			half4 _DeepColor;

			half4 _Range;
			
			
			v2f vert (appdata_full v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.aouv = v.texcoord;
				UNITY_TRANSFER_FOG(o,o.vertex);
				L_TRANSFER_SHADOWCOORDS(v, o);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				float3 worldTan = UnityObjectToWorldDir(v.tangent.xyz);
				float tanSign = v.tangent.w * unity_WorldTransformParams.w;
				float3 worldBinormal = cross(worldNormal, worldTan)*tanSign;
				o.RT0 = float4(worldTan.x, worldBinormal.x, worldNormal.x, worldPos.x);
				o.RT1 = float4(worldTan.y, worldBinormal.y, worldNormal.y, worldPos.y);
				o.RT2 = float4(worldTan.z, worldBinormal.z, worldNormal.z, worldPos.z);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 worldPos = float3(i.RT0.w,i.RT1.w,i.RT2.w);
				float3 rnormal = UnpackNormal(tex2D(_BumpTex, i.uv));
				float3 worldNormal = float3(dot(i.RT0.xyz, rnormal), dot(i.RT1.xyz, rnormal), dot(i.RT2.xyz, rnormal));

				L_SHADOW_ATTEN(atten, i);

				float3 litDir = normalize(GetLightDirection(worldPos.xyz));
				float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos.xyz));
				float3 h = normalize(viewDir + litDir);
				float ndl = max(0, dot(worldNormal, litDir));
				float spec = max(0, dot(worldNormal, h));
				float4 gi = texCUBElod(_GI, float4(worldNormal, _GILod))+ _GIColor;

				float3 ao = tex2D(_AOTex, i.aouv).rgb;

				fixed4 col = tex2D(_MainTex, i.uv);

				float clipArea = ClipArea(worldPos.xyz);
				float3 shallCol = lerp(float3(1, 1, 1), _ShallowColor.rgb, (1.0-saturate((worldPos.y - _Range.x) / _Range.y))*clipArea);
				float3 deepCol = lerp(float3(1, 1, 1), _DeepColor.rgb, (1.0 - saturate((worldPos.y - _Range.z) / _Range.w))*clipArea);

				float3 caustic = SampleCaustic(worldPos.xyz, clipArea); 

				col.rgb *= ao * shallCol*deepCol;

				col.rgb *= UNITY_LIGHTMODEL_AMBIENT.rgb + (caustic*ndl + internalWorldLightColor.rgb* ndl*gi.rgb + _SpecColor.rgb * pow(spec, _Specular)*_Gloss*tex2D(_SpecTex, i.uv).rgb*internalWorldLightColor.rgb) *atten;
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
				//return fixed4(depth, depth, depth, 1);
			}
			ENDCG
		}
	}
}
