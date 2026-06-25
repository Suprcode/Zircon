cbuffer EffectBuffer : register(b1)
{
    float4 SourceUV;
    float4 OutlineColour;
    float4 Effect;
    float4 Padding;
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

bool InsideSource(float2 uv)
{
    float2 sourceMin = min(SourceUV.xy, SourceUV.zw);
    float2 sourceMax = max(SourceUV.xy, SourceUV.zw);
    return uv.x >= sourceMin.x && uv.x <= sourceMax.x && uv.y >= sourceMin.y && uv.y <= sourceMax.y;
}

float4 SampleSprite(float2 uv)
{
    if (!InsideSource(uv))
        return float4(0, 0, 0, 0);

    return shaderTexture.Sample(sampleState, uv);
}

float4 PS_OUTLINE(PS_INPUT input) : SV_Target
{
    float2 textureSize = max(Effect.zw, float2(1.0, 1.0));
    float outlineThickness = max(Effect.y, 1.0);
    float2 texelSize = 1.0 / textureSize;

    float4 texColor = SampleSprite(input.Tex) * input.Col;
    float alpha = texColor.a;

    bool hasNeighbour = false;
    float minNeighbourDistance = outlineThickness + 1.0;
    int radius = (int)ceil(outlineThickness);

    for (int x = -radius; x <= radius; ++x)
    {
        for (int y = -radius; y <= radius; ++y)
        {
            if ((x == 0 && y == 0) || (abs(x) == radius && abs(y) == radius && radius == 0))
                continue;

            float2 offset = float2((float)x, (float)y) * texelSize;
            float neighbourAlpha = SampleSprite(input.Tex + offset).a;

            if (neighbourAlpha > 0.05)
            {
                hasNeighbour = true;
                float distance = length(float2((float)x, (float)y));
                minNeighbourDistance = min(minNeighbourDistance, distance);
            }
        }
    }

    if (alpha <= 0.05 && hasNeighbour)
    {
        float falloff = (outlineThickness <= 1.0) ? 0.0 : saturate((minNeighbourDistance - 1.0) / max(1.0, outlineThickness - 1.0));
        float outlineAlpha = lerp(1.0, 0.5, falloff);
        return float4(OutlineColour.rgb * outlineAlpha, outlineAlpha);
    }

    return float4(0, 0, 0, 0);
}
