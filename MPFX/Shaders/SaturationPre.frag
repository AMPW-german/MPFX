#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0, input_attachment_index = 0) uniform subpassInput Source;
layout(set = 1, binding = 1) uniform MPFXDefaultBufferAsset {
  float preAmount;
  float postAmount;
};

float luminance(vec3 c)
{
    return dot(c, vec3(0.2126, 0.7152, 0.0722));
}

vec3 saturate(vec3 color, float saturation)
{
    float l = luminance(color);
    return mix(color, vec3(l), saturation);
}

void main()
{
    vec4 c = subpassLoad(Source);
    outColor = vec4(saturate(c.rgb, 1 - preAmount), 1);
}
