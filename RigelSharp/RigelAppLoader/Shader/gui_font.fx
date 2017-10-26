struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float4 uv : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 uv : TEXCOORD0;
};

float4x4 worldViewProj;

Texture2D texfont;
SamplerState MeshTextureSampler;

PS_IN VS( VS_IN input )
{
	PS_IN output = (PS_IN)0;
	
	float4 pos = input.pos;
	output.pos = mul(pos, worldViewProj);
	output.col = input.col;
	output.uv = input.uv;
	return output;
}

float4 PS( PS_IN input ) : SV_Target
{
	float4 v = input.col;
	v.a = texfont.Sample(MeshTextureSampler,input.uv).r;
	return v;
}
