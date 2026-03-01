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
    vec4 newColor = vec4(0, 0, 0, 0);

    if (c.r == c.g)
    {
        if (c.r == c.b)
        {
            newColor = vec4(0, 0, 0, 1);
        }
        else if (c.r > c.b)
        {
            newColor = vec4(0, 0, c.b, 1);
        }
        else
        {
            newColor = vec4(c.r, c.g, 0, 1);
        }
    }
    else if (c.r == c.b)
    {

        if (c.r > c.g)
        {
            newColor = vec4(0, c.g, 0, 1);
        }
        else
        {
            newColor = vec4(c.r, 0, c.b, 1);
        }
    }
    else if (c.g == c.b)
    {
        if (c.g > c.r)
        {
            newColor = vec4(c.r, 0, 0, 1);
        }
        else
        {
            newColor = vec4(0, c.g, c.b, 1);
        }
    }
    else if (c.r > c.g && c.r > c.b)
    {
        newColor = vec4(0, c.g, c.b, 1);
    }
    else if (c.g > c.r && c.g > c.b)
    {
        newColor = vec4(c.r, 0, c.b, 1);
    }
    else if (c.b > c.r && c.b > c.g)
    {
        newColor = vec4(c.r, c.g, 0, 1);
    }
    else
    {
        newColor = c;
    }

    outColor = mix(c, newColor, preAmount);
}
