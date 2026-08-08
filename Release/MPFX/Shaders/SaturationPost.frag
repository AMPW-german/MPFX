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

layout(set = 1, binding = 2) uniform MPFXDefaultBufferAsset {
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
    vec4 c = texture(In, Uv);
    Out = vec4(saturate(c.rgb, 1 - postAmount), 1);
}
