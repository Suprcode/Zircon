cbuffer MatrixBuffer : register(b0)
{
    matrix Projection;
};

cbuffer OutlineBuffer : register(b1)
{
    float2 TexelSize;
    float Thickness;
    float Padding;
    float4 OutlineColor;
};

struct VS_INPUT
{
    float2 Pos : POSITION;
    float2 Tex : TEXCOORD;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Projection);
    output.Tex = input.Tex;
    return output;
}

Texture2D shaderTexture : register(t0);
SamplerState sampleState : register(s0);

float4 PS(PS_INPUT input) : SV_Target
{
    float4 baseColor = shaderTexture.Sample(sampleState, input.Tex);

    if (baseColor.a > 0.0001)
        return baseColor;

    float maxAlpha = 0.0;

    [unroll]
    for (int x = -4; x <= 4; ++x)
    {
        [unroll]
        for (int y = -4; y <= 4; ++y)
        {
            if (abs(x) == 0 && abs(y) == 0)
                continue;

            if (abs(x) > Thickness || abs(y) > Thickness)
                continue;

            float2 offset = input.Tex + float2(x * TexelSize.x, y * TexelSize.y);
            float alpha = shaderTexture.Sample(sampleState, offset).a;
            maxAlpha = max(maxAlpha, alpha);
        }
    }

    if (maxAlpha > 0.0001)
        return float4(OutlineColor.rgb, OutlineColor.a * maxAlpha);

    return float4(0, 0, 0, 0);
}
