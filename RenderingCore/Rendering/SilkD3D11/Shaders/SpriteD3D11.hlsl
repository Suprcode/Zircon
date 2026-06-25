cbuffer MatrixBuffer : register(b0)
{
    matrix Matrix;
};

struct VS_INPUT
{
    float2 Pos : POSITION;
    float2 Tex : TEXCOORD0;
    float4 Col : COLOR0;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD0;
    float4 Col : COLOR0;
    float2 ScreenPos : TEXCOORD1;
};

Texture2D shaderTexture : register(t0);
SamplerState sampleState : register(s0);

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Matrix);
    output.Tex = input.Tex;
    output.Col = input.Col;
    output.ScreenPos = input.Pos;
    return output;
}

float4 PS(PS_INPUT input) : SV_Target
{
    float4 texel = shaderTexture.Sample(sampleState, input.Tex);
    float alpha = texel.a * input.Col.a;
    return float4(texel.rgb * input.Col.rgb * input.Col.a, alpha);
}
