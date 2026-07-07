#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0, input_attachment_index = 0) uniform subpassInput Source;
layout(set = 1, binding = 1) uniform MPFXVec4BufferAsset {
  vec4 data; // preGrainSize, preGrainStrength, postGrainSize, postGrainStrength
  vec4 config; // width, height, framenumPre, framenumPost
};

layout(location = 0) in vec2 v_Uv;

layout(push_constant) uniform BlurParams {
    int radius;
    vec2 direction;
    float texelSize;
};
