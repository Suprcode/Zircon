float4x4 Matrix : register(c0);

float4 ShadowColor : register(c4);
float4 ShadowParams1 : register(c5); // xy = texture size, z = shadow width, w = start opacity
float4 ShadowParams2 : register(c6); // x = opacity exponent
float4 SourceUV : register(c7);      // xy = min, zw = max

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

sampler2D shaderTexture : register(s0);

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
    return tex2D(shaderTexture, clamped);
}

float4 SampleSpriteMasked(float2 uv, float2 sourceMin, float2 sourceMax)
{
    float4 colour = SampleSpriteClamped(uv, sourceMin, sourceMax);

    if (any(uv < sourceMin) || any(uv > sourceMax))
        colour.a = 0.0;

    return colour;
}

float4 PS_SHADOW(PS_INPUT input) : COLOR0
{
    float2 sourceMin = SourceUV.xy;
    float2 sourceMax = SourceUV.zw;
    float2 texelSize = 1.0 / ShadowParams1.xy;
    float2 clampedBase = clamp(input.Tex, sourceMin, sourceMax);

    // Draw the sprite normally when present
    float4 texColor = SampleSpriteMasked(input.Tex, sourceMin, sourceMax) * input.Col;
    if (texColor.a > 0.01)
        return texColor;

    // Otherwise, search for the nearest opaque texel within the shadow width
    float shadowWidth = ShadowParams1.z;
    float shadowMaxOpacity = ShadowParams1.w;
    float shadowOpacityExponent = max(ShadowParams2.x, 0.0001);

    float minDistance = shadowWidth + 1.0;
    bool hasNeighbour = false;

    static const int MAX_RADIUS = 8;
    int radius = (int)ceil(shadowWidth);
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
            float neighbourAlpha = SampleSpriteClamped(clampedBase + offset, sourceMin, sourceMax).a;

            if (neighbourAlpha > 0.05)
            {
                hasNeighbour = true;
                float distance = length(float2((float)x, (float)y));
                minDistance = min(minDistance, distance);
            }
        }
    }

    if (!hasNeighbour || minDistance > shadowWidth)
        return float4(0, 0, 0, 0);

    float t = saturate(minDistance / max(shadowWidth, 0.0001));
    float falloff = pow(1.0 - t, shadowOpacityExponent);
    float alpha = shadowMaxOpacity * falloff;

    return float4(ShadowColor.rgb, alpha);
}
