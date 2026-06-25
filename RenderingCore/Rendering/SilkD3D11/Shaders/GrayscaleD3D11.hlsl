struct PS_INPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD0;
    float4 Col : COLOR0;
    float2 ScreenPos : TEXCOORD1;
};

Texture2D shaderTexture : register(t0);
SamplerState sampleState : register(s0);

float4 PS_GRAY(PS_INPUT input) : SV_Target
{
    float4 texel = shaderTexture.Sample(sampleState, input.Tex);
    float gray = dot(texel.rgb, float3(0.299, 0.587, 0.114));
    float alpha = texel.a * input.Col.a;
    return float4(gray.xxx * input.Col.rgb * input.Col.a, alpha);
}
