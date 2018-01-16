Shader "Hidden/ForceRender"
{
	Properties
	{
		
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Tags {"RenderType"="Opaque"}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float height : TEXCOORD0;
			};

			uniform float internal_Force;

			v2f vert (appdata v)
			{
				v2f o;
				float4 camPos = mul(UNITY_MATRIX_MV, v.vertex);
				//o.height = step(0, 0 - camPos.z)*internal_Force;
				o.height = camPos.z;
				o.vertex = mul(UNITY_MATRIX_P, camPos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				if (i.height >= 0)
					return 0;
				float h = step(0, 0 - i.height)*internal_Force;
				return EncodeFloatRGBA(h);
			}
			ENDCG
		}
	}
}
