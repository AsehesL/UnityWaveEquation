Shader "Unlit/Water"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", color) = (1,1,1,1)
		_Noise ("Noise", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Transparent" }
		LOD 100

		GrabPass {}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 proj : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _GrabTexture;
			sampler2D _LiquidHeightMap;
			float4 _MainTex_ST;

			float4 _Color;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				o.proj = ComputeGrabScreenPos(o.vertex);
		
				return o;
			}

			float _Noise;
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				float4 heightMapCol = tex2D(_LiquidHeightMap, i.uv);
				float height;
				float3 normal;
				DecodeDepthNormal(heightMapCol, height, normal);

				float2 projUv = i.proj.xy / i.proj.w + normal.xy*_Noise;
				fixed4 col = tex2D(_GrabTexture, projUv);

				col.rgb *= _Color.rgb;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
