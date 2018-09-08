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
			sampler2D _PreTex;
			//sampler2D _ForceTex;

			float _Fade;

			half4 _WaveParams;

			fixed4 frag (v2f i) : SV_Target
			{
				//float force = DecodeFloatRGBA(tex2D(_ForceTex, i.uv));
				//float cur = _WaveParams.x*DecodeFloatRGBA(tex2D(_MainTex, i.uv)) + force;
				float cur = _WaveParams.x*tex2D(_MainTex, i.uv).r;
				
				float rg = _WaveParams.z*(tex2D(_MainTex, i.uv + float2(_WaveParams.w, 0)).r + tex2D(_MainTex, i.uv + float2(-_WaveParams.w,0)).r
				+ tex2D(_MainTex, i.uv + float2(0,_WaveParams.w)).r + tex2D(_MainTex, i.uv + float2(0,-_WaveParams.w)).r);

				float pre = _WaveParams.y*tex2D(_PreTex, i.uv).r;
				
				cur += rg + pre*_Fade;
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

			float _Offset;

			fixed4 frag(v2f i) : SV_Target
			{
				float ch = DecodeFloatRGBA(tex2D(_MainTex, i.uv));
				float lh = DecodeFloatRGBA(tex2D(_MainTex, i.uv + float2(-_Offset, 0.0)));
				float rh = DecodeFloatRGBA(tex2D(_MainTex, i.uv + float2(_Offset, 0.0)));
				float bh = DecodeFloatRGBA(tex2D(_MainTex, i.uv + float2(0.0, -_Offset)));
				float th = DecodeFloatRGBA(tex2D(_MainTex, i.uv + float2(0.0, _Offset)));

				float3 normal = normalize(float3(lh - rh, bh - th, 1.0));
				

				return EncodeDepthNormal(ch, normal);
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
				float4 col = tex2D(_MainTex, i.uv).r*0.45;
				col += tex2D(_MainTex, i.uv + _BlurOffset).r*0.15;
				col += tex2D(_MainTex, i.uv - _BlurOffset).r*0.15;
				col += tex2D(_MainTex, i.uv + _BlurOffset*2).r*0.125;
				col += tex2D(_MainTex, i.uv - _BlurOffset*2).r*0.125;

				return col;
			}
			ENDCG
		}
	}
}
