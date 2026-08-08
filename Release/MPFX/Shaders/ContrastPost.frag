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
    vec4 c = texture(In, Uv);
    Out = vec4(contrastHDR(c.rgb, postAmount), 1);
}
