#ifndef TEMP_CGINC
#define TEMP_CGINC

float4 TempEncode(float height) {
	float2 rg = EncodeFloatRG(height >= 0 ? height : 0);
	float2 ba = EncodeFloatRG(height < 0 ? -height : 0);

	return float4(rg, ba);
}

float TempDecode(float4 rgba) {
	float d1 = DecodeFloatRG(rgba.rg);
	float d2 = DecodeFloatRG(rgba.ba);

	if (d1 >= d2)
		return d1;
	else
		return -d2;
}

#endif