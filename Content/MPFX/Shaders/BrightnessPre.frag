#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0, input_attachment_index = 0) uniform subpassInput Source;
layout(set = 1, binding = 1) uniform MPFXDefaultBufferAsset {
  float preAmount;
  float postAmount;
};

void main()
{
  vec4 c = subpassLoad(Source);
  outColor = vec4(c.rgb * preAmount, 1);
}
