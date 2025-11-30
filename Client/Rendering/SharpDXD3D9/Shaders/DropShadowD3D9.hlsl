float4x4 Matrix : register(c0);

float4 ImgMinMax : register(c4);  // xy = min, zw = max
float4 ShadowParams : register(c5); // x = shadow size, y = max alpha

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
    float2 ScreenPos : TEXCOORD1;
};

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Matrix);
    output.Tex = input.Tex;
    output.Col = input.Col;
    output.ScreenPos = input.Pos;
    return output;
}

float4 PS_SHADOW(PS_INPUT input) : COLOR0
{
    float2 position = input.ScreenPos;

    float distLeft = ImgMinMax.x - position.x;
    float distTop = ImgMinMax.y - position.y;
    float distRight = position.x - ImgMinMax.z;
    float distBottom = position.y - ImgMinMax.w;

    float shadowDistance = max(max(distLeft, distTop), max(distRight, distBottom));

    if (shadowDistance <= 0.0)
        return float4(0, 0, 0, 0);

    float alpha = saturate(1.0 - shadowDistance / max(ShadowParams.x, 0.0001)) * ShadowParams.y;

    return float4(0, 0, 0, alpha);
}
