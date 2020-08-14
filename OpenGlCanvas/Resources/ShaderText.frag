#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform int index;
uniform vec4 color;

void main()
{
	float charWidth = 1/(97.0f);
	vec2 textCoordOfChar = vec2( (index * charWidth) + (texCoord.x * charWidth) - 0.003 ,texCoord.y);
    float a = texture(texture0, textCoordOfChar).a;
	outputColor = vec4(color.r,color.g,color.b,a);
}