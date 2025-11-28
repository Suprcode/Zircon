cbuffer MatrixBuffer : register(b0)
{
    matrix Matrix;
};

cbuffer DropShadowBuffer : register(b1)
{
    float4 ShadowColor;
    float2 TextureSize;
    float2 ShadowOffset;
    float ShadowBlur;
    float Padding;
    float4 SourceUV; // xy = min, zw = max
    float2 Padding2;
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

float SampleBaseAlpha(float2 uv, float2 sourceMin, float2 sourceMax)
{
    // Only consider alpha within the sprite's UV bounds to avoid suppressing the shadow outside the image.
    if (any(uv < sourceMin) || any(uv > sourceMax))
        return 0.0;

    return shaderTexture.Sample(sampleState, uv).a;
}

float SampleShadowAlpha(float2 uv, float2 sourceMin, float2 sourceMax)
{
    // Clamp to the sprite region so the shadow can sample the edge texels while still ignoring atlas neighbors.
    float2 clampedUv = clamp(uv, sourceMin, sourceMax);
    return shaderTexture.Sample(sampleState, clampedUv).a;
}

float4 PS_DROPSHADOW(PS_INPUT input) : SV_Target
{
    float2 sourceMin = SourceUV.xy;
    float2 sourceMax = SourceUV.zw;
    float2 texelSize = 1.0 / TextureSize;

    float baseAlpha = SampleBaseAlpha(input.Tex, sourceMin, sourceMax);
    if (baseAlpha > 0.05)
        return float4(0, 0, 0, 0);

    float blur = max(ShadowBlur, 0.01);
    int sampleRadius = (int)ceil(min(blur, 4.0));
    if (sampleRadius < 1) sampleRadius = 1;

    float2 shadowUv = input.Tex + ShadowOffset * texelSize;

    float alphaSum = 0.0;
    float weightSum = 0.0;
    float blurDenominator = max(1.0, blur * blur);

    for (int x = -sampleRadius; x <= sampleRadius; ++x)
    {
        for (int y = -sampleRadius; y <= sampleRadius; ++y)
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
