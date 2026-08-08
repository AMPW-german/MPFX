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
  mat4 preData;
  mat4 postData;
};

void main()
{
  vec3 c = texture(In, Uv).rgb;
  if (preData[3][3] == 0)
    Out = vec4(c, 1);
  else
  {
    c = (c + preData[0].rgb); // lift
    c = max(c, 0.0); // avoid negatives
    c = pow(c, 1.0 / preData[1].rgb); // gamma
    c *= preData[2].rgb; // gain
    c = min(c, 1.0);

    Out = vec4(c, 0.25);
  }
}