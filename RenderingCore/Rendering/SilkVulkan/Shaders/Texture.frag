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

vec4 MakeShadowPixel(vec4 shadow, float coverage, float shadowOpacity)
{
    float opacity = clamp(coverage, 0.0, 1.0) * shadowOpacity;
    vec3 colour = pushConstants.uTint.x > 0.5 ? shadow.rgb : shadow.rgb * opacity;
    return vec4(colour, shadow.a * opacity);
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
        vec4 left = LoadSourcePixel(pixel + ivec2(-1, 0), sourceMin, sourceMax);
        vec4 right = LoadSourcePixel(pixel + ivec2(1, 0), sourceMin, sourceMax);
        vec4 up = LoadSourcePixel(pixel + ivec2(0, -1), sourceMin, sourceMax);
        vec4 down = LoadSourcePixel(pixel + ivec2(0, 1), sourceMin, sourceMax);

        vec4 reference = IsDarkShadow(left) ? left :
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

        if (support < 2)
            return center;

        vec4 shadow = ((shadowLeft ? left : vec4(0.0)) + (shadowRight ? right : vec4(0.0)) +
                       (shadowUp ? up : vec4(0.0)) + (shadowDown ? down : vec4(0.0))) / float(support);

        int shapeSupport = support;
        shapeSupport += pixel.x == sourceMin.x ? 1 : 0;
        shapeSupport += pixel.x == sourceMax.x ? 1 : 0;
        shapeSupport += pixel.y == sourceMin.y ? 1 : 0;
        shapeSupport += pixel.y == sourceMax.y ? 1 : 0;
        return MakeShadowPixel(shadow, float(min(shapeSupport, 4)) * 0.25, shadowOpacity);
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

    int diagonalSupport = 0;
    diagonalSupport += ShadowSupport(topLeft, center);
    diagonalSupport += ShadowSupport(topRight, center);
    diagonalSupport += ShadowSupport(bottomLeft, center);
    diagonalSupport += ShadowSupport(bottomRight, center);

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
        return MakeShadowPixel(center, (1.0 + float(diagonalSupport)) * 0.2, shadowOpacity);
    }
    else
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

        float coverage = max(0.25, (1.0 + float(support)) / 9.0);
        return MakeShadowPixel(center, coverage, shadowOpacity);
    }
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
