#version 450 core

layout(location = 0) out vec4 Out;
layout(location = 0) in vec2 Uv;
layout(set = 1, binding = 0) uniform sampler2D In;

layout(std140, set = 1, binding = 1) uniform ShaderTime {
  uint FrameNumber;
  float DeltaTime;
  float RealTimeSinceStart;
  float TimeSinceStart;
  float TimeWarpSpeed;
} Time;

layout(set = 1, binding = 2) uniform MPFXColorBufferAsset {
  vec4 preAmount;
  vec4 postAmount;
};

void main()
{
  vec4 c = texture(In, Uv);
  Out = vec4(mix(c.rgb, postAmount.rgb, postAmount.a), 1);
}