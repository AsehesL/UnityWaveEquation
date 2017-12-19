Shader "Hidden/ForceRender"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Pos("Pos", vector) = (0,0,0,0)
		_Range("Range", float) = 0
		_Force("Force", float) = 0
		
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
			float4 _Pos;
			float _Range;
			float _Force;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				
				float dis = length(i.uv - _Pos.xy);
				float force = (1 - saturate(dis / _Range))*_Force;
				col.rgb += fixed3(force, force, force);

				return col;
			}
			ENDCG
		}
	}
}
