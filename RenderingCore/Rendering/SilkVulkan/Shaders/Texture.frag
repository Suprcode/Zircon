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

    vec4 texel = texture(uTexture, vTexCoord);

    if (effectMode == 1)
    {
        float gray = dot(texel.rgb, vec3(0.299, 0.587, 0.114));
        outColour = vec4(vec3(gray) * vColour.rgb * vColour.a,
                         texel.a * vColour.a);
        return;
    }

    outColour = vec4(texel.rgb * vColour.rgb * vColour.a,
                     texel.a * vColour.a);
}
