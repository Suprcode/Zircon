float4 OutlineColor : register(c0);
// x = 1/width, y = 1/height, z = thickness in pixels
float4 OutlineParams : register(c1);

sampler2D SpriteTexture : register(s0);

struct VS_INPUT
{
    float4 Pos : POSITION;
    float2 Tex : TEXCOORD0;
};

struct VS_OUTPUT
{
    float4 Pos : POSITION;
    float2 Tex : TEXCOORD0;
};

VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output;
    output.Pos = input.Pos;
    output.Tex = input.Tex;
    return output;
}

float4 PS(VS_OUTPUT input) : COLOR0
{
    float alpha = tex2D(SpriteTexture, input.Tex).a;
    if (alpha > 0.0f)
    {
        return float4(OutlineColor.rgb, OutlineColor.a);
    }

    float2 texel = OutlineParams.xy;
    float outline = OutlineParams.z;

    float2 offsets[8] = {
        float2(-1.0f, 0.0f),
        float2(1.0f, 0.0f),
        float2(0.0f, -1.0f),
        float2(0.0f, 1.0f),
        float2(-1.0f, -1.0f),
        float2(-1.0f, 1.0f),
        float2(1.0f, -1.0f),
        float2(1.0f, 1.0f)
    };

    [unroll]
    for (int i = 0; i < 8; i++)
    {
        float2 sampleUV = input.Tex + offsets[i] * texel * outline;
        alpha = tex2D(SpriteTexture, sampleUV).a;
        if (alpha > 0.0f)
        {
            return float4(OutlineColor.rgb, OutlineColor.a);
        }
    }

    return float4(0, 0, 0, 0);
}

technique OutlineSprite
{
    pass P0
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}
