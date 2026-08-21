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
    nointerpolation float4 Source : TEXCOORD3;
    nointerpolation float2 TextureSize : TEXCOORD4;
};

Texture2D shaderTextures[32] : register(t0);
SamplerState sampleState : register(s0);

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

float4 LoadTexture(uint textureIndex, int2 pixel)
{
    switch (textureIndex)
    {
        case 0: return shaderTextures[0].Load(int3(pixel, 0));
        case 1: return shaderTextures[1].Load(int3(pixel, 0));
        case 2: return shaderTextures[2].Load(int3(pixel, 0));
        case 3: return shaderTextures[3].Load(int3(pixel, 0));
        case 4: return shaderTextures[4].Load(int3(pixel, 0));
        case 5: return shaderTextures[5].Load(int3(pixel, 0));
        case 6: return shaderTextures[6].Load(int3(pixel, 0));
        case 7: return shaderTextures[7].Load(int3(pixel, 0));
        case 8: return shaderTextures[8].Load(int3(pixel, 0));
        case 9: return shaderTextures[9].Load(int3(pixel, 0));
        case 10: return shaderTextures[10].Load(int3(pixel, 0));
        case 11: return shaderTextures[11].Load(int3(pixel, 0));
        case 12: return shaderTextures[12].Load(int3(pixel, 0));
        case 13: return shaderTextures[13].Load(int3(pixel, 0));
        case 14: return shaderTextures[14].Load(int3(pixel, 0));
        case 15: return shaderTextures[15].Load(int3(pixel, 0));
        case 16: return shaderTextures[16].Load(int3(pixel, 0));
        case 17: return shaderTextures[17].Load(int3(pixel, 0));
        case 18: return shaderTextures[18].Load(int3(pixel, 0));
        case 19: return shaderTextures[19].Load(int3(pixel, 0));
        case 20: return shaderTextures[20].Load(int3(pixel, 0));
        case 21: return shaderTextures[21].Load(int3(pixel, 0));
        case 22: return shaderTextures[22].Load(int3(pixel, 0));
        case 23: return shaderTextures[23].Load(int3(pixel, 0));
        case 24: return shaderTextures[24].Load(int3(pixel, 0));
        case 25: return shaderTextures[25].Load(int3(pixel, 0));
        case 26: return shaderTextures[26].Load(int3(pixel, 0));
        case 27: return shaderTextures[27].Load(int3(pixel, 0));
        case 28: return shaderTextures[28].Load(int3(pixel, 0));
        case 29: return shaderTextures[29].Load(int3(pixel, 0));
        case 30: return shaderTextures[30].Load(int3(pixel, 0));
        default: return shaderTextures[31].Load(int3(pixel, 0));
    }
}

bool IsDarkShadow(float4 texel)
{
    return texel.a >= 0.75 && max(max(texel.r, texel.g), texel.b) <= 0.55;
}

bool IsSimilar(float4 first, float4 second)
{
    return length(first.rgb - second.rgb) <= 0.20;
}

bool IsTransparent(float4 texel)
{
    return texel.a <= 0.25;
}

float4 LoadSourcePixel(uint textureIndex, int2 pixel, int2 sourceMin, int2 sourceMax)
{
    if (pixel.x < sourceMin.x || pixel.y < sourceMin.y || pixel.x > sourceMax.x || pixel.y > sourceMax.y)
        return float4(0, 0, 0, 0);

    return LoadTexture(textureIndex, pixel);
}

bool IsSimilarShadow(float4 candidate, float4 reference)
{
    return IsDarkShadow(candidate) && IsSimilar(candidate, reference);
}

int ShadowSupport(float4 candidate, float4 reference)
{
    return IsSimilarShadow(candidate, reference) ? 1 : 0;
}

float4 MakeShadowPixel(float4 shadow, float coverage, float shadowOpacity)
{
    return float4(shadow.rgb, shadow.a * saturate(coverage) * shadowOpacity);
}

float4 FillShadowHatch(uint textureIndex, float2 uv, float4 spriteSource, float2 spriteTextureSize, float shadowOpacity)
{
    shadowOpacity = saturate(shadowOpacity);
    float4 center = SampleTexture(textureIndex, uv);
    bool centerTransparent = IsTransparent(center);
    if (!centerTransparent && !IsDarkShadow(center))
        return center;

    int2 textureSize = max(int2(spriteTextureSize), int2(1, 1));
    float2 sourceUvMin = min(spriteSource.xy, spriteSource.zw);
    float2 sourceUvMax = max(spriteSource.xy, spriteSource.zw);
    int2 sourceMin = clamp(int2(round(sourceUvMin * textureSize)), int2(0, 0), textureSize - 1);
    int2 sourceMax = clamp(int2(round(sourceUvMax * textureSize)) - 1, sourceMin, textureSize - 1);
    int2 pixel = clamp(int2(floor(uv * textureSize)), sourceMin, sourceMax);

    if (centerTransparent)
    {
        float4 left = LoadSourcePixel(textureIndex, pixel + int2(-1, 0), sourceMin, sourceMax);
        float4 right = LoadSourcePixel(textureIndex, pixel + int2(1, 0), sourceMin, sourceMax);
        float4 up = LoadSourcePixel(textureIndex, pixel + int2(0, -1), sourceMin, sourceMax);
        float4 down = LoadSourcePixel(textureIndex, pixel + int2(0, 1), sourceMin, sourceMax);

        float4 reference = IsDarkShadow(left) ? left :
                           IsDarkShadow(right) ? right :
                           IsDarkShadow(up) ? up : down;
        if (!IsDarkShadow(reference))
            return center;

        bool shadowLeft = IsSimilarShadow(left, reference);
        bool shadowRight = IsSimilarShadow(right, reference);
        bool shadowUp = IsSimilarShadow(up, reference);
        bool shadowDown = IsSimilarShadow(down, reference);
        int support = (shadowLeft ? 1 : 0) + (shadowRight ? 1 : 0) +
                      (shadowUp ? 1 : 0) + (shadowDown ? 1 : 0);
        bool touchesArtwork =
            (!IsTransparent(left) && !shadowLeft) ||
            (!IsTransparent(right) && !shadowRight) ||
            (!IsTransparent(up) && !shadowUp) ||
            (!IsTransparent(down) && !shadowDown);

        // A transparent checker sample has four possible shadow samples on the
        // opposite parity. Two are enough to establish a real hatch edge; the
        // fraction that remain is the reconstructed silhouette coverage.
        if (support < 2)
        {
            if (support == 1 && touchesArtwork)
                return MakeShadowPixel(reference, 1.0, shadowOpacity);

            return center;
        }

        float4 shadow = ((shadowLeft ? left : 0) + (shadowRight ? right : 0) +
                         (shadowUp ? up : 0) + (shadowDown ? down : 0)) / support;

        // The shadow/art join is not an outer silhouette. Keep it at the same
        // opacity as the resolved shadow so it cannot form a lighter seam.
        if (touchesArtwork)
            return MakeShadowPixel(shadow, 1.0, shadowOpacity);

        // A source-rectangle boundary is a batching/atlas boundary, not a
        // shadow silhouette. Once the local checker pattern is established,
        // extrapolate its missing samples so neighbouring map tiles do not get
        // a feathered line between them.
        int shapeSupport = support;
        shapeSupport += pixel.x == sourceMin.x ? 1 : 0;
        shapeSupport += pixel.x == sourceMax.x ? 1 : 0;
        shapeSupport += pixel.y == sourceMin.y ? 1 : 0;
        shapeSupport += pixel.y == sourceMax.y ? 1 : 0;
        return MakeShadowPixel(shadow, min(shapeSupport, 4) * 0.25, shadowOpacity);
    }

    float4 left = LoadSourcePixel(textureIndex, pixel + int2(-1, 0), sourceMin, sourceMax);
    float4 right = LoadSourcePixel(textureIndex, pixel + int2(1, 0), sourceMin, sourceMax);
    float4 up = LoadSourcePixel(textureIndex, pixel + int2(0, -1), sourceMin, sourceMax);
    float4 down = LoadSourcePixel(textureIndex, pixel + int2(0, 1), sourceMin, sourceMax);

    bool leftTransparent = IsTransparent(left);
    bool rightTransparent = IsTransparent(right);
    bool upTransparent = IsTransparent(up);
    bool downTransparent = IsTransparent(down);
    bool allTransparent = leftTransparent && rightTransparent && upTransparent && downTransparent;

    bool touchesArtwork =
        (!leftTransparent && !IsSimilarShadow(left, center)) ||
        (!rightTransparent && !IsSimilarShadow(right, center)) ||
        (!upTransparent && !IsSimilarShadow(up, center)) ||
        (!downTransparent && !IsSimilarShadow(down, center));

    if (!allTransparent && !touchesArtwork)
        return center;

    float4 topLeft = LoadSourcePixel(textureIndex, pixel + int2(-1, -1), sourceMin, sourceMax);
    float4 topRight = LoadSourcePixel(textureIndex, pixel + int2(1, -1), sourceMin, sourceMax);
    float4 bottomLeft = LoadSourcePixel(textureIndex, pixel + int2(-1, 1), sourceMin, sourceMax);
    float4 bottomRight = LoadSourcePixel(textureIndex, pixel + int2(1, 1), sourceMin, sourceMax);

    int diagonalSupport = 0;
    diagonalSupport += ShadowSupport(topLeft, center);
    diagonalSupport += ShadowSupport(topRight, center);
    diagonalSupport += ShadowSupport(bottomLeft, center);
    diagonalSupport += ShadowSupport(bottomRight, center);

    if (!allTransparent)
    {
        int transparentSupport = (leftTransparent ? 1 : 0) + (rightTransparent ? 1 : 0) +
                                 (upTransparent ? 1 : 0) + (downTransparent ? 1 : 0);

        // A shadow-coloured hatch pixel beside opaque artwork used to return
        // unchanged above, bypassing ShadowOpacity and producing a dark line.
        // Apply the normal resolved-shadow opacity once the diagonal hatch
        // confirms that this is shadow rather than artwork.
        if (touchesArtwork && transparentSupport >= 2 && diagonalSupport > 0)
            return MakeShadowPixel(center, 1.0, shadowOpacity);

        return center;
    }

    if (diagonalSupport > 0)
    {
        diagonalSupport += pixel.x == sourceMin.x || pixel.y == sourceMin.y ? 1 : 0;
        diagonalSupport += pixel.x == sourceMax.x || pixel.y == sourceMin.y ? 1 : 0;
        diagonalSupport += pixel.x == sourceMin.x || pixel.y == sourceMax.y ? 1 : 0;
        diagonalSupport += pixel.x == sourceMax.x || pixel.y == sourceMax.y ? 1 : 0;
        diagonalSupport = min(diagonalSupport, 4);
    }

    if (diagonalSupport >= 2)
    {
        // On this checker parity the 3x3 footprint contains the center and four
        // diagonals. Averaging their binary shape coverage feathers only the
        // silhouette; a fully surrounded hatch pixel remains unchanged.
        return MakeShadowPixel(center, (1.0 + diagonalSupport) * 0.2, shadowOpacity);
    }
    else
    {
        // At the silhouette the final covered hatch pixel may have no immediate
        // diagonal partner. Confirm it against the same two-pixel hatch grid so
        // it does not remain as an opaque spike.
        int support = 0;
        support += ShadowSupport(LoadSourcePixel(textureIndex, pixel + int2(-2, 0), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(textureIndex, pixel + int2(2, 0), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(textureIndex, pixel + int2(0, -2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(textureIndex, pixel + int2(0, 2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(textureIndex, pixel + int2(-2, -2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(textureIndex, pixel + int2(2, -2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(textureIndex, pixel + int2(-2, 2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(textureIndex, pixel + int2(2, 2), sourceMin, sourceMax), center);

        // The wider footprint handles narrow tips and diagonal endings without
        // letting a lone opaque checker pixel survive at full strength.
        float coverage = max(0.25, (1.0 + support) / 9.0);
        return MakeShadowPixel(center, coverage, shadowOpacity);
    }
}

float4 PS_SOLID_SHADOW(PS_INPUT input) : SV_Target
{
    uint textureIndex = (uint)round(input.TexIndex);
    float4 texel = FillShadowHatch(textureIndex, input.Tex, input.Source, input.TextureSize, Effect.y);
    float alpha = texel.a * input.Col.a;
    return float4(texel.rgb * input.Col.rgb * alpha, alpha);
}
