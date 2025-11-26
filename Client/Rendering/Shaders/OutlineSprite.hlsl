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

float4 SampleSprite(float2 uv, float2 sourceMin, float2 sourceMax)
{
    // Treat any sample outside the sprite's atlas rectangle as transparent. This keeps the outline aligned directly
    // against the sprite edge instead of shrinking it inward when padding is requested.
    if (any(uv < sourceMin) || any(uv > sourceMax))
        return float4(0, 0, 0, 0);

    return shaderTexture.Sample(sampleState, uv);
}

float4 PS_OUTLINE(PS_INPUT input) : SV_Target
{
    float2 sourceMin = SourceUV.xy;
    float2 sourceMax = SourceUV.zw;
    float2 texelSize = 1.0 / TextureSize;

    float4 texColor = SampleSprite(input.Tex, sourceMin, sourceMax) * input.Col;
    float alpha = texColor.a;

    bool hasNeighbour = false;
    int radius = (int)ceil(OutlineThickness);

    // Sample a square neighborhood sized by the requested thickness. This allows thicker borders (e.g., 10px)
    // to extend fully around the sprite instead of being clamped to a fixed 2px kernel.
    for (int x = -radius; x <= radius; ++x)
    {
        for (int y = -radius; y <= radius; ++y)
        {
            if ((x == 0 && y == 0) || (abs(x) == radius && abs(y) == radius && radius == 0))
                continue;

            float2 offset = float2((float)x, (float)y) * texelSize;
            float neighbourAlpha = SampleSprite(input.Tex + offset, sourceMin, sourceMax).a;

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
