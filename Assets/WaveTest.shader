Shader "Hidden/WaveTest"
{
	Properties
	{
		_CurTex("CurTex", 2D) = "white" {}
		_PreTex("PreTex", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
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
			
			sampler2D _CurTex;
			sampler2D _PreTex;

			half4 _WaveParams;

			fixed4 frag (v2f i) : SV_Target
			{
				float cur = _WaveParams.x*tex2D(_CurTex, i.uv).r;

			float rg = _WaveParams.z*(tex2D(_CurTex, i.uv + float2(_WaveParams.w, 0)).r + tex2D(_CurTex, i.uv + float2(-_WaveParams.w,0)).r
				+ tex2D(_CurTex, i.uv + float2(0,_WaveParams.w)).r + tex2D(_CurTex, i.uv + float2(0,-_WaveParams.w)).r);

			float pre = _WaveParams.y*tex2D(_PreTex, i.uv).r;

			cur += rg + pre;
				return fixed4(cur,cur,cur,1);
			}
			ENDCG
		}
	}
}
