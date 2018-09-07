Shader "Custom/NewSurfaceShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Boarder ("Boarder", vector) = (1,1,1,1)
		_Plane ("Plane", vector) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_HeightMap ("Height", 2D) = "" {}
		_Refract ("Refract", float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 shadowCoord;
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float4 _Boarder;
		float4 _Plane;
		sampler2D _HeightMap;

		float _Refract;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		void vert(inout appdata_full v, out Input IN) {
			UNITY_INITIALIZE_OUTPUT(Input, IN);

			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
			float3 worldNormal = UnityObjectToWorldNormal(v.normal);
			float t = (_Plane.w - dot(worldPos.xyz, _Plane.xyz)) / dot(worldNormal.xyz, _Plane.xyz);
			worldPos.xyz = worldPos.xyz + t*worldNormal.xyz;
			IN.shadowCoord.xy = ((worldPos.xz - _Boarder.xy) + _Boarder.zw) / (_Boarder.zw*2.0);
			IN.shadowCoord.z = t;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float height; float3 normal;

			DecodeDepthNormal(tex2D(_HeightMap, IN.shadowCoord.xy), height, normal);

			float3 refr = refract(-normal, IN.worldNormal, _Refract);

			float ndr = max(0, -dot(refr, _WorldSpaceLightPos0.xyz));

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb* _Color.rgb + c.rgb * ndr * 4;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
