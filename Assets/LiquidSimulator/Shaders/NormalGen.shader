Shader "Hidden/NormalGen"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Utils.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			fixed4 frag(v2f i) : SV_Target
			{
				float lh = DecodeHeight(tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x, 0.0)));
				float rh = DecodeHeight(tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x, 0.0)));
				float bh = DecodeHeight(tex2D(_MainTex, i.uv + float2(0.0, -_MainTex_TexelSize.y)));
				float th = DecodeHeight(tex2D(_MainTex, i.uv + float2(0.0, _MainTex_TexelSize.y)));

				//float3 va = normalize(float3(2.0, 0.0, rh - lh));
				//float3 vb = normalize(float3(0.0, 2.0, th - bh));

				float3 normal = normalize(float3(lh - rh, bh - th, 5.0*_MainTex_TexelSize.x));
				//float3 normal = cross(va, vb);


				//return EncodeDepthNormal(ch, normal);
#if defined(UNITY_NO_DXT5nm)
				return float4(normal*0.5 + 0.5, 1.0);
#else
#if UNITY_VERSION > 2018
				return float4(normal.x*0.5 + 0.5, normal.y*0.5 + 0.5, 0, 1); //2018修改了法线压缩方式，增加了一种BC5压缩
#else
				return float4(0, normal.y*0.5 + 0.5, 0, normal.x*0.5 + 0.5);
#endif
#endif
			}
			ENDCG
		}
	}
}
