float4x4 Matrix : register(c0);

float4 OutlineColor : register(c4);
float4 OutlineParams : register(c5); // xy = texture size, z = outline thickness, w = padding
float4 SourceUV : register(c6); // xy = min, zw = max

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

float4 SampleSprite(float2 uv, float2 sourceMin, float2 sourceMax)
{
    if (any(uv < sourceMin) || any(uv > sourceMax))
        return float4(0, 0, 0, 0);

    return tex2D(shaderTexture, uv);
}

float4 PS_OUTLINE(PS_INPUT input) : COLOR0
{
    float2 sourceMin = SourceUV.xy;
    float2 sourceMax = SourceUV.zw;
    float2 texelSize = 1.0 / OutlineParams.xy;

    float4 texColor = SampleSprite(input.Tex, sourceMin, sourceMax) * input.Col;
    float alpha = texColor.a;

    bool hasNeighbour = false;
    float outlineThickness = OutlineParams.z;
    float minNeighbourDistance = outlineThickness + 1.0;
    int radius = (int)ceil(outlineThickness);

    for (int x = -radius; x <= radius; ++x)
    {
        for (int y = -radius; y <= radius; ++y)
        {
            if (x == 0 && y == 0)
                continue;

            float2 offset = float2((float)x, (float)y) * texelSize;
            float neighbourAlpha = SampleSprite(input.Tex + offset, sourceMin, sourceMax).a;

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
        return float4(OutlineColor.rgb, outlineAlpha);
    }

    return float4(0, 0, 0, 0);
}
