Shader "Unlit/WaterBody"
{
	Properties
	{
		_BoundsMin("BoundsMin", vector) = (0,0,0,0)
		_BoundsMax("BoundsMax", vector) = (0,0,0,0)
		_FilterColor ("FilterColor", color) = (1,1,1,1)
		_LightIntensity ("Intensity", float) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" "IgnoreProjector" = "true" }
		LOD 100

		Pass
		{
			zwrite off
			blend srcalpha one
			colormask rgb
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#define RAY_STEP 64

			#include "UnityCG.cginc"
			#include "Utils.cginc"

			float3 _BoundsMin;
			float3 _BoundsMax;
			float4 _WaterPlane;

			float4 _FilterColor;
			float _LightIntensity;

			struct appdata {
				float4 vertex:POSITION;
				float2 texcoord:TEXCOORD0;
				float4 color:COLOR;
			};

			struct v2f
			{
				UNITY_FOG_COORDS(0)
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				float height = DecodeHeight(tex2Dlod(_LiquidHeightMap, float4(v.texcoord.xy, 0, 0)));
				v.vertex.y += height * (1 - v.color.a);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			float ClipInBounds(float3 worldPos) {
				if (worldPos.x < _BoundsMin.x || worldPos.x > _BoundsMax.x)
					return 0;
				if (worldPos.y < _BoundsMin.y || worldPos.y > _BoundsMax.y)
					return 0;
				if (worldPos.z < _BoundsMin.z || worldPos.z > _BoundsMax.z)
					return 0;
				return 1;
			}

			float SampleWaterNormalDotLightDir(float3 worldPos) {
				float3 boundSize = _BoundsMax - _BoundsMin;

				float3 lightDir = GetLightDirection(worldPos);
				float3 hitPos = worldPos + lightDir * (_WaterPlane.w - dot(worldPos, _WaterPlane.xyz) / dot(lightDir, _WaterPlane.xyz));
				float2 uv = (hitPos.xz - _BoundsMin.xz) / boundSize.xz;
				float3 normal = UnpackNormal(tex2D(_LiquidNormalMap, uv));
				return max(0, dot(normal, -lightDir));
			}
			
			float4 frag (v2f i) : SV_Target
			{
				float3 boundSize = _BoundsMax - _BoundsMin;
				float delta = max(boundSize.x, max(boundSize.y, boundSize.z)) / RAY_STEP;
				float coldelta = 1.0 / RAY_STEP * _LightIntensity;

				float3 viewDir = normalize(-UnityWorldSpaceViewDir(i.worldPos));

				float4 col = float4(0, 0, 0, 0);

				for (float k = 0; k < RAY_STEP; k++) {
					float3 wp = i.worldPos + viewDir * k * delta;
					float atten = SampleShadow(wp);
					float clipv = ClipInBounds(wp);

					float ndl = SampleWaterNormalDotLightDir(wp);

					col += _FilterColor * coldelta * atten * clipv*ndl;
				}

				return col;
			}
			ENDCG
		}
	}
}
