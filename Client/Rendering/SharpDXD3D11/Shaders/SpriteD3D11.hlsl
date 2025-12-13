cbuffer MatrixBuffer : register(b0)
{
    matrix Matrix;
};

struct VS_INPUT
{
    float2 Pos : POSITION;
    float2 Tex : TEXCOORD;
    float4 Col : COLOR;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD;
    float4 Col : COLOR;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    // Transform position by matrix (Projection * World)
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Matrix);
    output.Tex = input.Tex;
    output.Col = input.Col;
    return output;
}

Texture2D shaderTexture : register(t0);
SamplerState sampleState : register(s0);

float4 PS(PS_INPUT input) : SV_Target
{
    float4 texColor = shaderTexture.Sample(sampleState, input.Tex);
    return texColor * input.Col;
}