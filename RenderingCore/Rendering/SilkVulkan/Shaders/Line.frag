#version 450
layout(push_constant) uniform PushConstants
{
    vec2 uViewport;
    vec4 uColour;
} pushConstants;
layout(location = 0) out vec4 outColour;
void main()
{
    outColour = vec4(pushConstants.uColour.rgb * pushConstants.uColour.a,
                     pushConstants.uColour.a);
}
