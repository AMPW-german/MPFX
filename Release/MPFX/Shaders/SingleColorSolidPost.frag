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

void main()
{
    vec4 c = texture(In, Uv);
    vec4 newColor = vec4(0, 0, 0, 0);

    if (c.r == c.g)
    {
        if (c.r == c.b)
        {
            newColor = vec4(1, 1, 1, 1);
        }
        else if (c.r > c.b)
        {
            newColor = vec4(1, 1, 0, 1);
        }
        else
        {
            newColor = vec4(0, 0, 1, 1);
        }
    }
    else if (c.r == c.b)
    {

        if (c.r > c.g)
        {
            newColor = vec4(1, 0, 1, 1);
        }
        else
        {
            newColor = vec4(0, 1, 0, 1);
        }
    }
    else if (c.g == c.b)
    {

        if (c.g > c.r)
        {
            newColor = vec4(0, 1, 1, 1);
        }
        else
        {
            newColor = vec4(1, 0, 0, 1);
        }
    }
    else if (c.r > c.g && c.r > c.b)
    {
        newColor = vec4(1, 0, 0, 1);
    }
    else if (c.g > c.r && c.g > c.b)
    {
        newColor = vec4(0, 1, 0, 1);
    }
    else if (c.b > c.r && c.b > c.g)
    {
        newColor = vec4(0, 0, 1, 1);
    }
    else
    {
        newColor = c;
    }

    Out = mix(c, newColor, postAmount);
}
