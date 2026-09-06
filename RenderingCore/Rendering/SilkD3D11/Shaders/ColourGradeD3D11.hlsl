cbuffer EffectBuffer : register(b1)
{
    float4 GradeSettings;
    float4 GradeTint;
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

Texture2D shaderTexture : register(t0);
SamplerState sampleState : register(s0);

float4 PS_COLOUR_GRADE(PS_INPUT input) : SV_Target
{
    float4 texel = shaderTexture.Sample(sampleState, input.Tex);
    float alpha = texel.a * input.Col.a;
    float3 colour = texel.a > 0.0001 ? texel.rgb / texel.a : 0.0;

    colour *= exp2(GradeSettings.x);
    colour = (colour - 0.5) * GradeSettings.y + 0.5;

    float luminance = dot(colour, float3(0.2126, 0.7152, 0.0722));
    colour = lerp(luminance.xxx, colour, GradeSettings.z);

    float3 tint = lerp(1.0.xxx, GradeTint.rgb, GradeSettings.w);
    colour = saturate(colour * tint);

    return float4(colour * input.Col.rgb * alpha, alpha);
}
