cbuffer MatrixBuffer : register(b0)
{
    matrix Matrix;
};

cbuffer DropShadowBuffer : register(b1)
{
    float4 ShadowColor;          // rgb = colour, a = unused (alpha provided separately)
    float2 TextureSize;          // texture dimensions in pixels
    float ShadowWidth;           // shadow extension in pixels
    float ShadowMaxOpacity;      // starting opacity at the sprite edge
    float ShadowOpacityExponent; // controls how quickly the opacity falls off
    float3 Padding;              // padding for 16-byte alignment
    float4 SourceUV;             // xy = min, zw = max
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

float4 SampleSpriteClamped(float2 uv, float2 sourceMin, float2 sourceMax)
{
    float2 clamped = clamp(uv, sourceMin, sourceMax);
    return shaderTexture.Sample(sampleState, clamped);
}

float4 SampleSpriteMasked(float2 uv, float2 sourceMin, float2 sourceMax)
{
    float4 colour = SampleSpriteClamped(uv, sourceMin, sourceMax);

    if (any(uv < sourceMin) || any(uv > sourceMax))
        colour.a = 0.0;

    return colour;
}

float4 PS_SHADOW(PS_INPUT input) : SV_Target
{
    float2 sourceMin = SourceUV.xy;
    float2 sourceMax = SourceUV.zw;
    float2 texelSize = 1.0 / TextureSize;

    // Draw the sprite normally when present
    float4 texColor = SampleSpriteMasked(input.Tex, sourceMin, sourceMax) * input.Col;
    if (texColor.a > 0.01)
        return texColor;

    // Otherwise, search for the nearest opaque texel within the shadow width
    float minDistance = ShadowWidth + 1.0;
    bool hasNeighbour = false;

    static const int MAX_RADIUS = 8;
    int radius = (int)ceil(ShadowWidth);
    radius = min(radius, MAX_RADIUS);

    [unroll]
    for (int x = -MAX_RADIUS; x <= MAX_RADIUS; ++x)
    {
        [unroll]
        for (int y = -MAX_RADIUS; y <= MAX_RADIUS; ++y)
        {
            if (abs(x) > radius || abs(y) > radius || (x == 0 && y == 0))
                continue;

            float2 offset = float2((float)x, (float)y) * texelSize;
            float neighbourAlpha = SampleSpriteClamped(input.Tex + offset, sourceMin, sourceMax).a;

            if (neighbourAlpha > 0.05)
            {
                hasNeighbour = true;
                float distance = length(float2((float)x, (float)y));
                minDistance = min(minDistance, distance);
            }
        }
    }

    if (!hasNeighbour || minDistance > ShadowWidth)
        return float4(0, 0, 0, 0);

    // Smoothly fade from ShadowMaxOpacity at the sprite edge down to 0 at the edge of the shadow
    float t = saturate(minDistance / max(ShadowWidth, 0.0001));
    float falloff = pow(1.0 - t, max(ShadowOpacityExponent, 0.0001));
    float alpha = ShadowMaxOpacity * falloff;

    return float4(ShadowColor.rgb, alpha);
}
