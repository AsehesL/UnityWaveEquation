Shader "Hidden/WaveEquationGen"
{
	Properties
	{
		_MainTex ("MainTexture", 2D) = "white" {}
		_PreTex("PreTex", 2D) = "white" {}
		_WaveParams("WaveParams", vector) = (0,0,0,0)
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			//float4 _MainTex_TexelSize;
			sampler2D _PreTex;
			//sampler2D _ForceTex;

			//float _Fade;

			half4 _WaveParams;

			fixed4 frag (v2f i) : SV_Target
			{
				//float force = DecodeFloatRGBA(tex2D(_ForceTex, i.uv));
				//float cur = _WaveParams.x*DecodeFloatRGBA(tex2D(_MainTex, i.uv)) + force;
				float cur = _WaveParams.x*tex2D(_MainTex, i.uv).r;
				
				float rg = _WaveParams.z*(tex2D(_MainTex, i.uv + float2(_WaveParams.w, 0)).r + tex2D(_MainTex, i.uv + float2(-_WaveParams.w,0)).r
				+ tex2D(_MainTex, i.uv + float2(0, _WaveParams.w)).r + tex2D(_MainTex, i.uv + float2(0,-_WaveParams.w)).r);

				float pre = _WaveParams.y*tex2D(_PreTex, i.uv).r;
				
				cur += rg + pre;
				cur = saturate(cur);

				//return EncodeFloatRGBA(cur);
				return fixed4(cur, cur, cur, 1);
			}
			ENDCG
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

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

			//float _Offset;

			fixed4 frag(v2f i) : SV_Target
			{
				float ch = tex2D(_MainTex, i.uv).r * 60;
				float lh = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x*0.5, 0.0)).r * 60;
				float rh = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x*0.5, 0.0)).r * 60;
				float bh = tex2D(_MainTex, i.uv + float2(0.0, -_MainTex_TexelSize.y*0.5)).r * 60;
				float th = tex2D(_MainTex, i.uv + float2(0.0, _MainTex_TexelSize.y*0.5)).r * 60;

				float3 va = normalize(float3(2.0, 0.0, rh - lh));
				float3 vb = normalize(float3(0.0, 2.0, th - bh));

				//float3 normal = normalize(float3(lh - rh, bh - th, 1.0));
				float3 normal = cross(va, vb);
				

				//return EncodeDepthNormal(ch, normal);
#if defined(UNITY_NO_DXT5nm)
				return float4(normal*0.5 + 0.5, 1.0);
#else
				return float4(0, normal.y*0.5+0.5, 0, normal.x*0.5+0.5);
#endif
			}
			ENDCG
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

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

			float2 _BlurOffset;

			fixed4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv)*0.45;
				col += tex2D(_MainTex, i.uv + _BlurOffset)*0.15;
				col += tex2D(_MainTex, i.uv - _BlurOffset)*0.15;
				col += tex2D(_MainTex, i.uv + _BlurOffset*2)*0.125;
				col += tex2D(_MainTex, i.uv - _BlurOffset*2)*0.125;

				return col;
			}
			ENDCG
		}
	}
}
