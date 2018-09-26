// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Caustic"
{
	Properties
	{
		_Height ("Height", float) = 1
		_Refract ("Refract", float) = 0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
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
				float2 oldPos : TEXCOORD1;
				float2 newPos : TEXCOORD2;
			};

			//sampler2D _LiquidHeightMap;
			sampler2D _LiquidNormalMap;

			half _Height;
			half _Refract;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				float3 normal = UnpackNormal(tex2Dlod(_LiquidNormalMap, float4(v.texcoord.xy, 0, 0)));
				
				float3 binormal = cross(normalize(v.normal), normalize(v.tangent.xyz)) * v.tangent.w;
				//float3x3 rotationToObj = float3x3(float3(v.tangent.x, binormal.x, v.normal.x), float3(v.tangent.y, binormal.y, v.normal.y), float3(v.tangent.z, binormal.z, v.normal.z));
				//float3 worldNormal = float3(dot(float3(v.tangent.x, binormal.x, v.normal.x), normal), dot(float3(v.tangent.y, binormal.y, v.normal.y), normal), dot(float3(v.tangent.z, binormal.z, v.normal.z), normal));
				float3 worldNormal = float3(normal.x, normal.z, normal.y);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

				float3 lightDir = UnityWorldSpaceLightDir(worldPos);

				//worldNormal = float3(dot(float3(worldTan.x, worldBinormal.x, worldNormal.x), normal), dot(float3(worldTan.y, worldBinormal.y, worldNormal.y), normal), dot(float3(worldTan.z, worldBinormal.z, worldNormal.z), normal));

				float3 refDir = refract(-lightDir, worldNormal.xyz, _Refract);

				float4 plane = float4(0, 1, 0, dot(float3(0,1,0), mul(unity_ObjectToWorld, float4(0, v.vertex.y+_Height, 0, 1).xyz)));
				float4 origPlane = float4(0, -1, 0, dot(float3(0, -1, 0), mul(unity_ObjectToWorld, float4(0, v.vertex.y, 0, 1).xyz)));
				float3 refpos = worldPos + refDir * (plane.w - dot(worldPos, plane.xyz) / dot(refDir, plane.xyz));


				float4 vertex;
				vertex.xyz = refpos + lightDir * (origPlane.w - dot(refpos, origPlane.xyz) / dot(lightDir, origPlane.xyz));
				vertex.w = 1.0;

				float2 offsetDir = vertex.xz - v.vertex.xz;
				float len = min(length(offsetDir), 0.05);
				vertex.xz = v.vertex.xz + normalize(offsetDir)*len;

				o.oldPos = v.vertex.xz;
				o.newPos = vertex.xz;

				o.vertex = UnityObjectToClipPos(vertex);//UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float oldArea = length(ddx(i.oldPos)) * length(ddy(i.oldPos));
				float newArea = length(ddx(i.newPos)) * length(ddy(i.newPos));

				float area = (oldArea / newArea) * 0.2;

				return float4(area, area, area, 1);
			}
			ENDCG
		}
	}
}
