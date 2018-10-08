Shader "Hidden/TestGizmos"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 100

		Pass
		{
			cull off
			zwrite off
			ztest always
			blend srcalpha oneminussrcalpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float4 color : COLOR;
				float4 vertex : SV_POSITION;
			};
			
			v2f vert (float4 vertex:POSITION, float4 color:COLOR)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.color = color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
