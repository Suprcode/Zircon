#version 450
layout(location = 0) in vec4 iPositionX;
layout(location = 1) in vec4 iPositionY;
layout(location = 2) in vec4 iTexCoordU;
layout(location = 3) in vec4 iTexCoordV;
layout(location = 4) in vec4 iColour;
layout(push_constant) uniform PushConstants
{
    vec2 uViewport;
    vec4 uTint;
    vec4 uSource;
    vec4 uOutlineColour;
    vec4 uEffect;
} pushConstants;
layout(location = 0) out vec2 vTexCoord;
layout(location = 1) out vec4 vColour;
layout(location = 2) out vec2 vScreenPos;
void main()
{
    int corner = gl_VertexIndex == 0 ? 0 :
                 gl_VertexIndex == 1 ? 1 :
                 gl_VertexIndex == 2 ? 2 :
                 gl_VertexIndex == 3 ? 0 :
                 gl_VertexIndex == 4 ? 2 : 3;
    vec2 position = vec2(iPositionX[corner], iPositionY[corner]);
    vec2 ndc = vec2((position.x / pushConstants.uViewport.x) * 2.0 - 1.0,
                    (position.y / pushConstants.uViewport.y) * 2.0 - 1.0);
    gl_Position = vec4(ndc, 0.0, 1.0);
    vTexCoord = vec2(iTexCoordU[corner], iTexCoordV[corner]);
    vColour = iColour;
    vScreenPos = position;
}
