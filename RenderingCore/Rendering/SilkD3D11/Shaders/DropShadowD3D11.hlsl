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
    float TexIndex : TEXCOORD1;
    float2 ScreenPos : TEXCOORD2;
};

float4 PS_SHADOW(PS_INPUT input) : SV_Target
{
    float2 position = input.ScreenPos;

    float2 imgMin = SourceUV.xy;
    float2 imgMax = SourceUV.zw;

    float distLeft = imgMin.x - position.x;
    float distTop = imgMin.y - position.y;
    float distRight = position.x - imgMax.x;
    float distBottom = position.y - imgMax.y;

    float shadowDistance = max(max(distLeft, distTop), max(distRight, distBottom));

    // Keep the boundary covered: at fractional scales it can lie on a pixel
    // centre excluded by the image's bottom/right rasterization edges. Allow
    // a tiny overlap (in screen pixels) for interpolation roundoff as well.
    float2 pixelSize = fwidth(position);
    float edgeTolerance = 0.001 * max(pixelSize.x, pixelSize.y);
    if (shadowDistance < -edgeTolerance)
        return float4(0, 0, 0, 0);

    float shadowSize = max(Effect.y, 0.0001);
    float maxAlpha = Effect.z;
    float alpha = saturate(1.0 - max(shadowDistance, 0.0) / shadowSize) * maxAlpha;

    return float4(0, 0, 0, alpha);
}
