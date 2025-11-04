#version 330 core
in vec2 TexCoords;
out vec4 FragColor;

uniform sampler2D screenTexture;  // 输入默认帧缓冲区的纹理

void main()
{
    // 高斯模糊（5x5 核）
    float kernel[25] = float[](
        1.0, 4.0, 6.0, 4.0, 1.0,
        4.0, 16.0, 24.0, 16.0, 4.0,
        6.0, 24.0, 36.0, 24.0, 6.0,
        4.0, 16.0, 24.0, 16.0, 4.0,
        1.0, 4.0, 6.0, 4.0, 1.0
    );
    vec2 texelSize = 1.0 / textureSize(screenTexture, 0);
    vec3 result = vec3(0.0);
    int index = 0;
    for(int i = -2; i <= 2; i++)
    {
        for(int j = -2; j <= 2; j++)
        {
            result += texture(screenTexture, TexCoords + vec2(i, j) * texelSize).rgb * kernel[index];
            index++;
        }
    }
    // 高斯核的总权重已经是256，但额外除以256以调整亮度
    FragColor = vec4(result / 256.0, 1.0);
}