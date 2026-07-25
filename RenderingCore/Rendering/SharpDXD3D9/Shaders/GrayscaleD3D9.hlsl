
float4x4 Matrix : register(c0);

struct VS_INPUT
{
    float2 Pos : POSITION0;
    float2 Tex : TEXCOORD0;
    float4 Col : COLOR0;
};

struct PS_INPUT
{
    float4 Pos : POSITION0;
    float2 Tex : TEXCOORD0;
    float4 Col : COLOR0;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Matrix);
    output.Tex = input.Tex;
    output.Col = input.Col;
    return output;
}

sampler2D shaderTexture : register(s0);

float4 PS_GRAY(PS_INPUT input) : COLOR0
{
    float4 texColor = tex2D(shaderTexture, input.Tex);
    float gray = dot(texColor.rgb, float3(0.299f, 0.587f, 0.114f));

    // Preserve the source alpha so grayscale doesn't introduce additional opacity.
    return float4(gray * input.Col.rgb, texColor.a);
}
