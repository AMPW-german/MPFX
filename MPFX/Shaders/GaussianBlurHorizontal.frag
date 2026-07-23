#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0) uniform sampler2D Source;

layout(location = 0) in vec2 Uv;

layout(std140, set = 1, binding = 1) uniform BlurParams {
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
      outColor = texture(Source, Uv);
      return;
  }

  vec4 color = texture(Source, Uv) * getWeight(0);
  vec2 texelSize = 1.0 / vec2(textureSize(Source, 0));

  vec2 minUv = 0.5 * texelSize;
  vec2 maxUv = 1.0 - minUv;

  for (int i = 1; i <= blur.radius; i++) {
    vec2 offset = vec2(float(i) * texelSize.x, 0.0);

    color += texture(Source, clamp(Uv + offset, minUv, maxUv)) * getWeight(i);
    color += texture(Source, clamp(Uv - offset, minUv, maxUv)) * getWeight(i);
  }

  outColor = color;
}
