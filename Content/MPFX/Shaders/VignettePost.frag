#version 450 core

layout(location = 0) out vec4 outColor;

layout(set = 1, binding = 0, input_attachment_index = 0) uniform subpassInput Source;

// Data: outer circle, inner circle, aspect ratio
layout(set = 1, binding = 1) uniform DataBuffer {
  vec4 postData;
  vec4 postColor;
};

layout(location = 0) in vec2 v_Uv;


void main()
{
  vec4 c = subpassLoad(Source);

  if (postData.z == 0)
  {
    outColor = c;
  }
  else
  {
    vec2 uv2 = v_Uv;
    uv2 = (uv2 - 0.5) * 2; // normalize to [-1, 1]
    uv2.x = uv2.x / postData.z; // adjust for screen size

    float d = sqrt(uv2.x * uv2.x + uv2.y * uv2.y); // distance from center
    d = clamp(d, postData.y, postData.x);
    d = d - postData.y;
    d = d / (postData.x - postData.y);

    d = clamp(d, 0, 1);

    outColor = vec4(mix(c.rgb, postColor.rgb, d * postColor.a), 1);
  }
}