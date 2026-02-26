#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0, input_attachment_index = 0) uniform subpassInput Source;
layout(set = 1, binding = 1) uniform ColorBuffer {
  vec4 preAmount;
  vec4 postAmount;
};

void main()
{
  vec4 c = subpassLoad(Source);
  outColor = vec4(mix(c.rgb, preAmount.rgb, preAmount.a), 1);
}