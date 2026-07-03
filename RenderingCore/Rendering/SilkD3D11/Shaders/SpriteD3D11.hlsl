cbuffer MatrixBuffer : register(b0)
{
    matrix Matrix;
};

struct VS_INPUT
{
    float2 Pos : POSITION;
    float2 Tex : TEXCOORD0;
    float4 Col : COLOR0;
    float TexIndex : TEXCOORD1;
};

struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD0;
    float4 Col : COLOR0;
    float TexIndex : TEXCOORD1;
    float2 ScreenPos : TEXCOORD2;
};

Texture2D shaderTextures[32] : register(t0);
SamplerState sampleState : register(s0);

PS_INPUT VS(VS_INPUT input)
{
    PS_INPUT output;
    output.Pos = mul(float4(input.Pos, 0.0, 1.0), Matrix);
    output.Tex = input.Tex;
    output.Col = input.Col;
    output.TexIndex = input.TexIndex;
    output.ScreenPos = input.Pos;
    return output;
}

float4 SampleTexture(uint textureIndex, float2 uv)
{
    switch (textureIndex)
    {
        case 0: return shaderTextures[0].Sample(sampleState, uv);
        case 1: return shaderTextures[1].Sample(sampleState, uv);
        case 2: return shaderTextures[2].Sample(sampleState, uv);
        case 3: return shaderTextures[3].Sample(sampleState, uv);
        case 4: return shaderTextures[4].Sample(sampleState, uv);
        case 5: return shaderTextures[5].Sample(sampleState, uv);
        case 6: return shaderTextures[6].Sample(sampleState, uv);
        case 7: return shaderTextures[7].Sample(sampleState, uv);
        case 8: return shaderTextures[8].Sample(sampleState, uv);
        case 9: return shaderTextures[9].Sample(sampleState, uv);
        case 10: return shaderTextures[10].Sample(sampleState, uv);
        case 11: return shaderTextures[11].Sample(sampleState, uv);
        case 12: return shaderTextures[12].Sample(sampleState, uv);
        case 13: return shaderTextures[13].Sample(sampleState, uv);
        case 14: return shaderTextures[14].Sample(sampleState, uv);
        case 15: return shaderTextures[15].Sample(sampleState, uv);
        case 16: return shaderTextures[16].Sample(sampleState, uv);
        case 17: return shaderTextures[17].Sample(sampleState, uv);
        case 18: return shaderTextures[18].Sample(sampleState, uv);
        case 19: return shaderTextures[19].Sample(sampleState, uv);
        case 20: return shaderTextures[20].Sample(sampleState, uv);
        case 21: return shaderTextures[21].Sample(sampleState, uv);
        case 22: return shaderTextures[22].Sample(sampleState, uv);
        case 23: return shaderTextures[23].Sample(sampleState, uv);
        case 24: return shaderTextures[24].Sample(sampleState, uv);
        case 25: return shaderTextures[25].Sample(sampleState, uv);
        case 26: return shaderTextures[26].Sample(sampleState, uv);
        case 27: return shaderTextures[27].Sample(sampleState, uv);
        case 28: return shaderTextures[28].Sample(sampleState, uv);
        case 29: return shaderTextures[29].Sample(sampleState, uv);
        case 30: return shaderTextures[30].Sample(sampleState, uv);
        default: return shaderTextures[31].Sample(sampleState, uv);
    }
}

float4 PS(PS_INPUT input) : SV_Target
{
    uint textureIndex = (uint)round(input.TexIndex);
    float4 texel = SampleTexture(textureIndex, input.Tex);
    float alpha = texel.a * input.Col.a;
    return float4(texel.rgb * input.Col.rgb * alpha, alpha);
}
