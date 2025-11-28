float4x4 Matrix : register(c0);

float4 ShadowColor : register(c4);
float4 ShadowParams1 : register(c5); // xy = texture size, z = blur radius
float4 ShadowParams2 : register(c6); // xy = shadow offset in pixels
float4 SourceUV : register(c7); // xy = min, zw = max

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

float SampleBaseAlpha(float2 uv, float2 sourceMin, float2 sourceMax)
{
    // Only consider alpha within the sprite's UV bounds to avoid suppressing the shadow outside the image.
    if (any(uv < sourceMin) || any(uv > sourceMax))
        return 0.0;

    return tex2D(shaderTexture, uv).a;
}

float SampleShadowAlpha(float2 uv, float2 sourceMin, float2 sourceMax)
{
    // Clamp to the sprite region so the shadow can sample the edge texels while still ignoring atlas neighbors.
    float2 clampedUv = clamp(uv, sourceMin, sourceMax);
    return tex2D(shaderTexture, clampedUv).a;
}

float4 PS_DROPSHADOW(PS_INPUT input) : COLOR0
{
    float2 sourceMin = SourceUV.xy;
    float2 sourceMax = SourceUV.zw;
    float2 texelSize = 1.0 / ShadowParams1.xy;

    float baseAlpha = SampleBaseAlpha(input.Tex, sourceMin, sourceMax);
    if (baseAlpha > 0.05)
        return float4(0, 0, 0, 0);

    float blur = max(ShadowParams1.z, 0.01);
    float2 shadowUv = input.Tex + ShadowParams2.xy * texelSize;

    // Use a small, fixed kernel suitable for ps_3_0
    static const int RADIUS = 3;
    float alphaSum = 0.0;
    float weightSum = 0.0;
    float sigma = max(blur * 0.55, 0.75);
    float blurDenominator = max(1.0, 2.0 * sigma * sigma);

    [unroll]
    for (int x = -RADIUS; x <= RADIUS; ++x)
    {
        [unroll]
        for (int y = -RADIUS; y <= RADIUS; ++y)
        {
            float2 offset = float2((float)x, (float)y) * texelSize;
            float sampleAlpha = SampleShadowAlpha(shadowUv + offset, sourceMin, sourceMax);
            float distance = length(float2((float)x, (float)y));
            float weight = exp(-distance * distance / blurDenominator);

            alphaSum += sampleAlpha * weight;
            weightSum += weight;
        }
    }

    float shadowAlpha = (weightSum > 0.0) ? alphaSum / weightSum : 0.0;
    return float4(ShadowColor.rgb, shadowAlpha * ShadowColor.a);
}
