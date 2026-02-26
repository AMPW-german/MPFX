#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0, input_attachment_index = 0) uniform subpassInput Source;

void main()
{
    vec4 c = subpassLoad(Source);

    if (c.r == c.g)
    {
        if (c.r == c.b)
        {
            outColor = vec4(1, 1, 1, 1);
        }
        else if (c.r > c.b)
        {
            outColor = vec4(1, 1, 0, 1);
        }
        else
        {
            outColor = vec4(0, 0, 1, 1);
        }
    }
    else if (c.r == c.b)
    {

        if (c.r > c.g)
        {
            outColor = vec4(1, 0, 1, 1);
        }
        else
        {
            outColor = vec4(0, 1, 0, 1);
        }
    }
    else if (c.g == c.b)
    {

        if (c.g > c.r)
        {
            outColor = vec4(0, 1, 1, 1);
        }
        else
        {
            outColor = vec4(1, 0, 0, 1);
        }
    }
    else if (c.r > c.g && c.r > c.b)
    {
        outColor = vec4(1, 0, 0, 1);
    }
    else if (c.g > c.r && c.g > c.b)
    {
        outColor = vec4(0, 1, 0, 1);
    }
    else if (c.b > c.r && c.b > c.g)
    {
        outColor = vec4(0, 0, 1, 1);
    }
    else
    {
        outColor = c;
    }
}
