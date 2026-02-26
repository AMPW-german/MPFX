#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0, input_attachment_index = 0) uniform subpassInput Source;
layout(set = 1, binding = 1) uniform MPFXDefaultBufferAsset {
  float preAmount;
  float postAmount;
};

vec3 contrastSDR(vec3 color, float contrast)
{
    return 0.5 + contrast * (color - 0.5);
}

vec3 contrastHDR(vec3 color, float contrast)
{
    const float midGray = 0.18;
    return midGray + contrast * (color - midGray);
}

void main()
{
    vec4 c = subpassLoad(Source);
    outColor = vec4(contrastHDR(c.rgb, preAmount), 1);
}
