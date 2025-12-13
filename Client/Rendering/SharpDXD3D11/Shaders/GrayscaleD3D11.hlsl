struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD;
    float4 Col : COLOR;
};

Texture2D shaderTexture : register(t0);
SamplerState sampleState : register(s0);

float4 PS_GRAY(PS_INPUT input) : SV_Target
{
    float4 texColor = shaderTexture.Sample(sampleState, input.Tex);
    float gray = dot(texColor.rgb, float3(0.299f, 0.587f, 0.114f));

    // Preserve the source alpha so grayscale doesn't introduce additional opacity.
    return float4(gray * input.Col.rgb, texColor.a);
}