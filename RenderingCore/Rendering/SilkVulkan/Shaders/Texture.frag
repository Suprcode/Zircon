#version 450
layout(set = 0, binding = 0) uniform sampler2D uTexture;
layout(push_constant) uniform PushConstants
{
    vec2 uViewport;
    vec4 uTint;
    vec4 uSource;
    vec4 uOutlineColour;
    vec4 uEffect;
} pushConstants;
layout(location = 0) in vec2 vTexCoord;
layout(location = 1) in vec4 vColour;
layout(location = 2) in vec2 vScreenPos;
layout(location = 3) flat in vec4 vSource;
layout(location = 0) out vec4 outColour;

bool InsideSource(vec2 uv)
{
    vec2 sourceMin = min(pushConstants.uSource.xy, pushConstants.uSource.zw);
    vec2 sourceMax = max(pushConstants.uSource.xy, pushConstants.uSource.zw);
    return uv.x >= sourceMin.x && uv.x <= sourceMax.x &&
           uv.y >= sourceMin.y && uv.y <= sourceMax.y;
}

vec4 SampleSprite(vec2 uv)
{
    if (!InsideSource(uv))
        return vec4(0.0);

    return texture(uTexture, uv);
}

bool IsDarkShadow(vec4 texel)
{
    return texel.a >= 0.75 && max(max(texel.r, texel.g), texel.b) <= 0.55;
}

bool IsSimilar(vec4 first, vec4 second)
{
    return length(first.rgb - second.rgb) <= 0.20;
}

bool IsTransparent(vec4 texel)
{
    return texel.a <= 0.25;
}

vec4 LoadSourcePixel(ivec2 pixel, ivec2 sourceMin, ivec2 sourceMax)
{
    if (pixel.x < sourceMin.x || pixel.y < sourceMin.y || pixel.x > sourceMax.x || pixel.y > sourceMax.y)
        return vec4(0.0);

    return texelFetch(uTexture, pixel, 0);
}

bool IsSimilarShadow(vec4 candidate, vec4 reference)
{
    return IsDarkShadow(candidate) && IsSimilar(candidate, reference);
}

int ShadowSupport(vec4 candidate, vec4 reference)
{
    return IsSimilarShadow(candidate, reference) ? 1 : 0;
}

vec4 MakeShadowCoverage(vec4 first, vec4 second, vec4 third, vec4 fourth, float shadowOpacity)
{
    float alphaSum = first.a + second.a + third.a + fourth.a;
    float alpha = alphaSum * shadowOpacity * 0.25;
    vec3 colour;
    if (pushConstants.uTint.x > 0.5)
    {
        colour = (first.rgb * first.a + second.rgb * second.a + third.rgb * third.a + fourth.rgb * fourth.a) /
                 max(alphaSum, 0.001);
    }
    else
    {
        colour = (first.rgb + second.rgb + third.rgb + fourth.rgb) * shadowOpacity * 0.25;
    }

    return vec4(colour, alpha);
}

vec4 MakeCoveredShadow(vec4 center, float shadowOpacity)
{
    vec3 colour = pushConstants.uTint.x > 0.5 ? center.rgb : center.rgb * shadowOpacity;
    return vec4(colour, center.a * shadowOpacity);
}

vec4 FillShadowHatch(vec2 uv)
{
    float shadowOpacity = clamp(pushConstants.uEffect.y, 0.0, 1.0);
    vec4 center = texture(uTexture, uv);
    bool centerTransparent = IsTransparent(center);
    if (!centerTransparent && !IsDarkShadow(center))
        return center;

    ivec2 textureSize = max(ivec2(pushConstants.uEffect.zw), ivec2(1));
    vec2 sourceUvMin = min(vSource.xy, vSource.zw);
    vec2 sourceUvMax = max(vSource.xy, vSource.zw);
    ivec2 sourceMin = clamp(ivec2(round(sourceUvMin * vec2(textureSize))), ivec2(0), textureSize - 1);
    ivec2 sourceMax = clamp(ivec2(round(sourceUvMax * vec2(textureSize))) - 1, sourceMin, textureSize - 1);
    ivec2 pixel = clamp(ivec2(floor(uv * vec2(textureSize))), sourceMin, sourceMax);

    if (centerTransparent)
    {
        bool missingLeft = pixel.x == sourceMin.x;
        bool missingRight = pixel.x == sourceMax.x;
        bool missingUp = pixel.y == sourceMin.y;
        bool missingDown = pixel.y == sourceMax.y;
        int missingCount = (missingLeft ? 1 : 0) + (missingRight ? 1 : 0) +
                           (missingUp ? 1 : 0) + (missingDown ? 1 : 0);
        if (missingCount > 1)
            return center;

        vec4 reference = vec4(0.0);
        bool hasReference = false;
        vec4 left = reference;
        vec4 right = reference;
        vec4 up = reference;
        vec4 down = reference;

        if (!missingLeft)
        {
            left = LoadSourcePixel(pixel + ivec2(-1, 0), sourceMin, sourceMax);
            if (!IsDarkShadow(left)) return center;
            reference = left;
            hasReference = true;
        }
        if (!missingRight)
        {
            right = LoadSourcePixel(pixel + ivec2(1, 0), sourceMin, sourceMax);
            if (!IsDarkShadow(right) || (hasReference && !IsSimilar(right, reference))) return center;
            if (!hasReference) { reference = right; hasReference = true; }
        }
        if (!missingUp)
        {
            up = LoadSourcePixel(pixel + ivec2(0, -1), sourceMin, sourceMax);
            if (!IsDarkShadow(up) || (hasReference && !IsSimilar(up, reference))) return center;
            if (!hasReference) { reference = up; hasReference = true; }
        }
        if (!missingDown)
        {
            down = LoadSourcePixel(pixel + ivec2(0, 1), sourceMin, sourceMax);
            if (!IsDarkShadow(down) || (hasReference && !IsSimilar(down, reference))) return center;
            if (!hasReference) { reference = down; hasReference = true; }
        }

        return MakeShadowCoverage(missingLeft ? reference : left, missingRight ? reference : right,
                                  missingUp ? reference : up, missingDown ? reference : down, shadowOpacity);
    }

    vec4 left = LoadSourcePixel(pixel + ivec2(-1, 0), sourceMin, sourceMax);
    if (!IsTransparent(left)) return center;
    vec4 right = LoadSourcePixel(pixel + ivec2(1, 0), sourceMin, sourceMax);
    if (!IsTransparent(right)) return center;
    vec4 up = LoadSourcePixel(pixel + ivec2(0, -1), sourceMin, sourceMax);
    if (!IsTransparent(up)) return center;
    vec4 down = LoadSourcePixel(pixel + ivec2(0, 1), sourceMin, sourceMax);
    if (!IsTransparent(down)) return center;

    vec4 topLeft = LoadSourcePixel(pixel + ivec2(-1, -1), sourceMin, sourceMax);
    vec4 topRight = LoadSourcePixel(pixel + ivec2(1, -1), sourceMin, sourceMax);
    vec4 bottomLeft = LoadSourcePixel(pixel + ivec2(-1, 1), sourceMin, sourceMax);
    vec4 bottomRight = LoadSourcePixel(pixel + ivec2(1, 1), sourceMin, sourceMax);

    bool diagonalA = IsSimilarShadow(topLeft, center) && IsSimilarShadow(bottomRight, center);
    bool diagonalB = IsSimilarShadow(topRight, center) && IsSimilarShadow(bottomLeft, center);
    bool edgeDiagonal =
        (pixel.x == sourceMin.x && IsSimilarShadow(topRight, center) && IsSimilarShadow(bottomRight, center)) ||
        (pixel.x == sourceMax.x && IsSimilarShadow(topLeft, center) && IsSimilarShadow(bottomLeft, center)) ||
        (pixel.y == sourceMin.y && IsSimilarShadow(bottomLeft, center) && IsSimilarShadow(bottomRight, center)) ||
        (pixel.y == sourceMax.y && IsSimilarShadow(topLeft, center) && IsSimilarShadow(topRight, center));
    if (!diagonalA && !diagonalB && !edgeDiagonal)
    {
        int support = 0;
        support += ShadowSupport(LoadSourcePixel(pixel + ivec2(-2, 0), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(pixel + ivec2(2, 0), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(pixel + ivec2(0, -2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(pixel + ivec2(0, 2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(pixel + ivec2(-2, -2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(pixel + ivec2(2, -2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(pixel + ivec2(-2, 2), sourceMin, sourceMax), center);
        support += ShadowSupport(LoadSourcePixel(pixel + ivec2(2, 2), sourceMin, sourceMax), center);

        float coverage = shadowOpacity * (support == 0 ? 0.25 : (support == 1 ? 0.5 : 1.0));
        vec3 colour = pushConstants.uTint.x > 0.5 ? center.rgb : center.rgb * coverage;
        return vec4(colour, center.a * coverage);
    }

    return MakeCoveredShadow(center, shadowOpacity);
}

void main()
{
    int effectMode = int(pushConstants.uEffect.x + 0.5);

    if (effectMode == 2)
    {
        vec4 center = SampleSprite(vTexCoord);
        if (center.a > 0.01)
            discard;

        vec2 textureSize = max(pushConstants.uEffect.zw, vec2(1.0));
        float thickness = max(pushConstants.uEffect.y, 1.0);
        vec2 texel = 1.0 / textureSize;
        bool hasNeighbour = false;
        float minNeighbourDistance = thickness + 1.0;
        int radius = int(ceil(thickness));

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x == 0 && y == 0)
                    continue;

                vec2 offset = vec2(float(x), float(y));
                if (SampleSprite(vTexCoord + offset * texel).a > 0.05)
                {
                    hasNeighbour = true;
                    minNeighbourDistance = min(minNeighbourDistance, length(offset));
                }
            }
        }

        if (!hasNeighbour)
            discard;

        float falloff = thickness <= 1.0 ? 0.0 : clamp((minNeighbourDistance - 1.0) / max(1.0, thickness - 1.0), 0.0, 1.0);
        float outlineAlpha = mix(1.0, 0.5, falloff) * pushConstants.uOutlineColour.a * vColour.a;
        outColour = vec4(pushConstants.uOutlineColour.rgb * outlineAlpha, outlineAlpha);
        return;
    }

    if (effectMode == 3)
    {
        vec4 bounds = pushConstants.uSource;
        vec2 boundsMin = min(bounds.xy, bounds.zw);
        vec2 boundsMax = max(bounds.xy, bounds.zw);

        float distLeft = boundsMin.x - vScreenPos.x;
        float distTop = boundsMin.y - vScreenPos.y;
        float distRight = vScreenPos.x - boundsMax.x;
        float distBottom = vScreenPos.y - boundsMax.y;
        float shadowDistance = max(max(distLeft, distTop), max(distRight, distBottom));

        if (shadowDistance <= 0.0)
            discard;

        float shadowSize = max(pushConstants.uEffect.y, 0.0001);
        float maxAlpha = pushConstants.uEffect.z;
        float alpha = clamp(1.0 - shadowDistance / shadowSize, 0.0, 1.0) * maxAlpha * pushConstants.uOutlineColour.a * vColour.a;

        outColour = vec4(pushConstants.uOutlineColour.rgb * alpha, alpha);
        return;
    }

    vec4 texel = effectMode == 4 ? FillShadowHatch(vTexCoord) : texture(uTexture, vTexCoord);
    float sourceAlpha = pushConstants.uTint.x > 0.5 ? texel.a : 1.0;

    if (effectMode == 1)
    {
        float gray = dot(texel.rgb, vec3(0.299, 0.587, 0.114));
        outColour = vec4(vec3(gray) * vColour.rgb * vColour.a * sourceAlpha,
                         texel.a * vColour.a);
        return;
    }

    outColour = vec4(texel.rgb * vColour.rgb * vColour.a * sourceAlpha,
                     texel.a * vColour.a);
}
