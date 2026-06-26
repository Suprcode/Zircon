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
    float2 position = input.Pos.xy;

    float2 imgMin = SourceUV.xy;
    float2 imgMax = SourceUV.zw;

    float distLeft = imgMin.x - position.x;
    float distTop = imgMin.y - position.y;
    float distRight = position.x - imgMax.x;
    float distBottom = position.y - imgMax.y;

    float shadowDistance = max(max(distLeft, distTop), max(distRight, distBottom));

    if (shadowDistance <= 0.0)
        return float4(0, 0, 0, 0);

    float shadowSize = max(Effect.y, 0.0001);
    float maxAlpha = Effect.z;
    float alpha = saturate(1.0 - shadowDistance / shadowSize) * maxAlpha;

    return float4(0, 0, 0, alpha);
}
