#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0, input_attachment_index = 0) uniform subpassInput Source;

// Data: outer circle, inner circle, aspect ratio
layout(set = 1, binding = 1) uniform DataBuffer {
  mat4 preData;
  mat4 postData;
};

void main()
{
  vec3 color = subpassLoad(Source).rgb;
  if (postData[3][3] == 0)
    outColor = vec4(color, 1);
  else
  {
    color = (color + postData[0].rgb); // lift
    color = max(color, 0.0); // avoid negatives
    color = pow(color, 1.0 / postData[1].rgb); // gamma
    color *= postData[2].rgb; // gain
    color = min(color, 1.0);

    outColor = vec4(color, 0.25);
  }
}