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

// Data: outer circle, inner circle, aspect ratio
layout(set = 1, binding = 2) uniform DataBuffer {
  vec4 preData;
  vec4 preColor;
};

void main()
{
  vec4 c = texture(In, Uv);

  if (preData.z == 0)
  {
    Out = c;
  }
  else
  {
    vec2 uv2 = Uv;
    uv2 = (uv2 - 0.5) * 2; // normalize to [-1, 1]
    uv2.x = uv2.x / preData.z; // adjust for screen size

    float d = sqrt(uv2.x * uv2.x + uv2.y * uv2.y); // distance from center
    d = clamp(d, preData.y, preData.x);
    d = d - preData.y;
    d = d / (preData.x - preData.y);

    d = clamp(d, 0, 1);

    Out = vec4(mix(c.rgb, preColor.rgb, d * preColor.a), 1);
  }
}