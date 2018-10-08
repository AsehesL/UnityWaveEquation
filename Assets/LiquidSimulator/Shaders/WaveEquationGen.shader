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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _PreTex;


			half4 _WaveParams;

			fixed4 frag (v2f i) : SV_Target
			{
				float cur = _WaveParams.x*DecodeHeight(tex2D(_MainTex, i.uv));
				
				float rg = _WaveParams.z*(DecodeHeight(tex2D(_MainTex, i.uv + float2(_WaveParams.w, 0))) + DecodeHeight(tex2D(_MainTex, i.uv + float2(-_WaveParams.w,0)))
				+ DecodeHeight(tex2D(_MainTex, i.uv + float2(0, _WaveParams.w))) + DecodeHeight(tex2D(_MainTex, i.uv + float2(0,-_WaveParams.w))));

				float pre = _WaveParams.y*DecodeHeight(tex2D(_PreTex, i.uv));
				
				cur += rg + pre;

				cur *= 0.96;

				return EncodeHeight(cur);
			}
			ENDCG
		}
	}
}
