cbuffer MatrixBuffer : register(b0)
{
    matrix Matrix;
};

cbuffer OutlineBuffer : register(b1)
{
    float4 OutlineColor;
    float2 TextureSize;
    float OutlineThickness;
    float Padding;
    float4 SourceUV; // xy = min, zw = max
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

Texture2D shaderTexture : register(t0);
SamplerState sampleState : register(s0);

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Matrix);
    output.Tex = input.Tex;
    output.Col = input.Col;
    return output;
}

float4 SampleSprite(float2 uv, float2 sourceMin, float2 sourceMax, float2 paddingUv)
{
    // Compute virtual padding directly in UV space so sampling respects the original atlas location.
    // If padding would eclipse the sprite completely, collapse to a degenerate inner rectangle but keep it valid.
    float2 innerMin = min(sourceMin + paddingUv, sourceMax);
    float2 innerMax = max(sourceMax - paddingUv, innerMin + float2(0.0001, 0.0001));

    // Treat any sample outside the padded inner rect as transparent, even if it would clamp to opaque edge texels.
    if (any(uv < innerMin) || any(uv > innerMax))
        return float4(0, 0, 0, 0);

    return shaderTexture.Sample(sampleState, uv);
}

float4 PS_OUTLINE(PS_INPUT input) : SV_Target
{
    float2 paddingUv = (Padding > 0.0) ? (Padding / TextureSize) : 0.0;
    float2 sourceMin = SourceUV.xy;
    float2 sourceMax = SourceUV.zw;
    float2 texelSize = 1.0 / TextureSize;

    float4 texColor = SampleSprite(input.Tex, sourceMin, sourceMax, paddingUv) * input.Col;
    float alpha = texColor.a;

    bool hasNeighbour = false;
    int radius = (int)OutlineThickness;

    [unroll]
    for (int x = -2; x <= 2; ++x)
    {
        [unroll]
        for (int y = -2; y <= 2; ++y)
        {
            if (abs(x) > radius || abs(y) > radius || (x == 0 && y == 0))
                continue;

            float2 offset = float2(x, y) * texelSize;
            float neighbourAlpha = SampleSprite(input.Tex + offset, sourceMin, sourceMax, paddingUv).a;

            if (neighbourAlpha > 0.05)
            {
                hasNeighbour = true;
            }
        }
    }

    // Suppress drawing of interior pixels here so the main sprite can be rendered normally in a subsequent pass.
    // The outline shader is now responsible only for emitting the border.
    if (alpha <= 0.05 && hasNeighbour)
    {
        // Draw a fully opaque outline regardless of the incoming vertex color or global opacity.
        return float4(OutlineColor.rgb, 1.0);
    }

    return float4(0, 0, 0, 0);
}
