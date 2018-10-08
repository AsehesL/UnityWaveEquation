// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Caustic"
{
	Properties
	{
		_Refract ("Refract", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 oldPos : TEXCOORD1;
				float2 newPos : TEXCOORD2;
			};

			half _Refract;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				float3 normal = UnpackNormal(tex2Dlod(_LiquidNormalMap, float4(v.texcoord.xy, 0, 0)));

				o.oldPos = v.vertex.xz;
				v.vertex.xz += normal.xy*_Refract;
				o.newPos = v.vertex.xz;
				

				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float oldArea = length(ddx(i.oldPos)) * length(ddy(i.oldPos));
				float newArea = length(ddx(i.newPos)) * length(ddy(i.newPos));

				float area = (oldArea / newArea) * 0.5;

				return float4(area, area, area, 1);
			}
			ENDCG
		}
	}
}
