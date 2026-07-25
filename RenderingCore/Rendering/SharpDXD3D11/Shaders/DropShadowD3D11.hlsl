cbuffer MatrixBuffer : register(b0)
{
    matrix Matrix;
};

cbuffer DropShadowBuffer : register(b1)
{
    float2 ImgMin;      // Top-left corner of the image in screen pixels
    float2 ImgMax;      // Bottom-right corner of the image in screen pixels
    float ShadowSize;   // Shadow extension in pixels
    float MaxAlpha;     // Opacity at the image edge
    float2 Padding;     // Padding for 16-byte alignment
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

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Matrix);
    output.Tex = input.Tex;
    output.Col = input.Col;
    return output;
}

float4 PS_SHADOW(PS_INPUT input) : SV_Target
{
    float2 position = input.Pos.xy;

    float distLeft = ImgMin.x - position.x;
    float distTop = ImgMin.y - position.y;
    float distRight = position.x - ImgMax.x;
    float distBottom = position.y - ImgMax.y;

    float shadowDistance = max(max(distLeft, distTop), max(distRight, distBottom));

    if (shadowDistance <= 0.0)
        return float4(0, 0, 0, 0);

    float alpha = saturate(1.0 - shadowDistance / max(ShadowSize, 0.0001)) * MaxAlpha;

    return float4(0, 0, 0, alpha);
}
