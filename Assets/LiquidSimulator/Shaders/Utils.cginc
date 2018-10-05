#ifndef UTILS_CGINC
#define UTILS_CGINC

uniform float4 internalWorldLightPos;
uniform float4 internalWorldLightColor;

sampler2D internalShadowMap;
float4x4 internalWorldLightMV;
float4x4 internalWorldLightVP;
float4 internalProjectionParams;
float internalBias;

float3 GetLightDirection(float3 worldPos) {
	if (internalWorldLightPos.w == 0)
		return -internalWorldLightPos.xyz;
	else
		return internalWorldLightPos.xyz - worldPos;
}

#define L_SHADOWCOORDS(n, m) float4 shadowProj:TEXCOORD##n;float shadowDepth:TEXCOORD##m;

#define L_TRANSFER_SHADOWCOORDS(v, o) \
	float4 cpos = mul(internalWorldLightMV, mul(unity_ObjectToWorld, v.vertex)); \
	o.shadowProj = mul(internalWorldLightVP, cpos); \
	float4 pj = o.shadowProj * 0.5f; \
	pj.xy = float2(pj.x, pj.y) + pj.w; \
	pj.zw = o.shadowProj.zw; \
	o.shadowProj = pj; \
	o.shadowDepth = -cpos.z*internalProjectionParams.w

#define L_SHADOW_ATTEN(atten, input) \
	float4 shadow = tex2Dproj(internalShadowMap, input.shadowProj); \
	float shadowdepth = DecodeFloatRGBA(shadow); \
	float shadowcol = step(input.shadowDepth - internalBias, shadowdepth)*0.7 + 0.3; \
	float2 shadowatten = saturate((0.5 - abs(input.shadowProj.xy / input.shadowProj.w - 0.5)) / (1 - internalWorldLightColor.a)); \
	float atten = shadowatten.x*shadowatten.y*shadowcol

#define L_SHADOW_ATTEN_REFRACT(atten, input, refract) \
	float2 sduv = input.shadowProj.xy/input.shadowProj.w + refract; \
	float4 shadow = tex2D(internalShadowMap, sduv); \
	float shadowdepth = DecodeFloatRGBA(shadow); \
	float shadowcol = step(input.shadowDepth - internalBias, shadowdepth)*0.7 + 0.3; \
	float2 shadowatten = saturate((0.5 - abs(input.shadowProj.xy / input.shadowProj.w - 0.5)) / (1 - internalWorldLightColor.a)); \
	float atten = shadowatten.x*shadowatten.y*shadowcol

float SampleShadow(float3 worldPos) {
	float4 cpos = mul(internalWorldLightMV, float4(worldPos, 1.0f));
	float4 shadowProj = mul(internalWorldLightVP, cpos); 
	float4 pj = shadowProj * 0.5f;
	pj.xy = float2(pj.x, pj.y) + pj.w; 
	pj.zw = shadowProj.zw;
	shadowProj = pj;
	float depth = -cpos.z*internalProjectionParams.w;
	float4 shadow = tex2Dproj(internalShadowMap, shadowProj);
	float shadowdepth = DecodeFloatRGBA(shadow); 
	float shadowcol = step(depth - internalBias, shadowdepth);
	//float2 shadowatten = saturate((0.5 - abs(shadowProj.xy / shadowProj.w - 0.5)) / (1 - internalWorldLightColor.a));
	//float atten = shadowatten.x*shadowatten.y*shadowcol;
	return shadowcol;
}

sampler2D _LiquidHeightMap;
sampler2D _LiquidNormalMap;
sampler2D _LiquidReflectMap;
sampler2D _CausticMap;

float4 _CausticPlane;
float4 _CausticRange;
float2 _CausticDepthRange;
float _CausticIntensity;

float4 _LiquidArea;

float ClipArea(float3 worldPos) {
	/*if (worldPos.x > _LiquidArea.x&&worldPos.x < _LiquidArea.z)
		return 0;
	return 1;*/

	float x = 1.0 - step(_LiquidArea.x, worldPos.x)*step(worldPos.x, _LiquidArea.z);
	float y = 1.0 - step(_LiquidArea.y, worldPos.z)*step(worldPos.z, _LiquidArea.w);
	return 1.0 - saturate(x + y);
	//float x = step(_LiquidArea.z, step(worldPos.x, _LiquidArea.x));
	//return x;
}

float3 SampleCaustic(float3 worldPos, float clipArea) {
	float3 lightDir = GetLightDirection(worldPos);
	float3 hitPos = worldPos + lightDir * (_CausticPlane.w - dot(worldPos, _CausticPlane.xyz) / dot(lightDir, _CausticPlane.xyz));
	float2 uv = (hitPos.xz - _CausticRange.xy) / _CausticRange.zw*0.5 + 0.5;
	float fade = 1.0 - saturate((worldPos.y - _CausticDepthRange.x) / _CausticDepthRange.y);
	float3 caustic = tex2D(_CausticMap, uv).rgb - 0.5;
	if (uv.x < 0 || uv.x>1 || uv.y < 0 || uv.y>1)
		return 0;
	return caustic*clipArea*fade*_CausticIntensity;
}

float4 EncodeHeight(float height) {
	float2 rg = EncodeFloatRG(height >= 0 ? height : 0);
	float2 ba = EncodeFloatRG(height < 0 ? -height : 0);

	return float4(rg, ba);
}

float DecodeHeight(float4 rgba) {
	float d1 = DecodeFloatRG(rgba.rg);
	float d2 = DecodeFloatRG(rgba.ba);

	if (d1 >= d2)
		return d1;
	else
		return -d2;
}

#endif