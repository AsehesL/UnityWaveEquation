Shader "Hidden/ForceTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ForcePos ("ForcePos", vector) = (0,0,0,0)
		_Force ("Force", float) = 0
		_ForceRange("ForceRange", float) = 0
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
			
			sampler2D _MainTex;

			half2 _ForcePos;
			half _Force;
			half _ForceRange;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float l = _Force*(1-saturate(length(i.uv - _ForcePos)/_ForceRange));

				col.rgb += l;

				return col;
			}
			ENDCG
		}
	}
}
