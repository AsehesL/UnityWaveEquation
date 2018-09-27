Shader "Custom/UnderWaterTest" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Height ("Height", vector) = (0,0,0,0)
		_UnderColor ("UnderColor", color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf UnderWaterLight fullforwardshadows

		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		fixed4 _Color;

		float2 _Height;
		float4 _UnderColor;

		float4 _CausticRange;
		float4 _CausticPlane;
		sampler2D _CausticMap;

		half4 LightingUnderWaterLight(SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir);
			half4 c;

			//float3 hitPos = refpos + lightDir * (origPlane.w - dot(refpos, origPlane.xyz) / dot(lightDir, origPlane.xyz));

			c.rgb = s.Albedo * _LightColor0.rgb * (NdotL * atten);
			c.a = s.Alpha;
			return c;
		}

		UNITY_INSTANCING_CBUFFER_START(Props)
		UNITY_INSTANCING_CBUFFER_END

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			float3 underCol = lerp(_UnderColor.rgb, float3(1,1,1), saturate((IN.worldPos.y-_Height.x) / _Height.y));

			float3 lightDir = UnityWorldSpaceLightDir(IN.worldPos);
			float3 hitPos = IN.worldPos.xyz + lightDir * (_CausticPlane.w - dot(IN.worldPos, _CausticPlane.xyz) / dot(lightDir, _CausticPlane.xyz));
			float2 uv = (hitPos.xz - _CausticRange.xy) / _CausticRange.zw*0.5 + 0.5;
			float4 causticCol = tex2D(_CausticMap, uv);

			o.Albedo = c.rgb * underCol + causticCol.rgb;

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
