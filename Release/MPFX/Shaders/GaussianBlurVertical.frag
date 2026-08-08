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

layout(std140, set = 1, binding = 2) uniform BlurParams {
  int radius;
  vec4 weights[6];
} blur;

float getWeight(int index)
{
  // index >> 2 == index / 4
  // index & 3 == index % 4
  return blur.weights[index >> 2][index & 3];
}

void main()
{
  if (blur.radius <= 0) 
  {
      Out = texture(In, Uv);
      return;
  }

  vec4 c = texture(In, Uv) * getWeight(0);
  vec2 texelSize = 1.0 / vec2(textureSize(In, 0));

  vec2 minUv = 0.5 * texelSize;
  vec2 maxUv = 1.0 - minUv;

  for (int i = 1; i <= blur.radius; i++) {
    vec2 offset = vec2(0.0, float(i) * texelSize.y);

    c += texture(In, clamp(Uv + offset, minUv, maxUv)) * getWeight(i);
    c += texture(In, clamp(Uv - offset, minUv, maxUv)) * getWeight(i);
  }

  Out = c;
}
