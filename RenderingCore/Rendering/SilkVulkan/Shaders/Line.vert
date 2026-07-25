#version 450
layout(location = 0) in vec2 aPosition;
layout(push_constant) uniform PushConstants
{
    vec2 uViewport;
    vec4 uColour;
} pushConstants;
void main()
{
    vec2 ndc = vec2((aPosition.x / pushConstants.uViewport.x) * 2.0 - 1.0,
                    (aPosition.y / pushConstants.uViewport.y) * 2.0 - 1.0);
    gl_Position = vec4(ndc, 0.0, 1.0);
}
