#version 330 core
out vec4 FragColor;

in vec2 TexCoords;
in vec3 Normal;       // 世界空间原始法线
in vec3 Position;     // 世界空间片段位置

// 外部参数
uniform vec3 cameraPos;         // 相机世界位置
uniform samplerCube skybox;     // 天空盒贴图

// 材质贴图（遵循常见命名规范）
uniform sampler2D texture_diffuse1;   // 漫反射颜色
uniform sampler2D texture_specular1;  // 粗糙度（值越高越粗糙）
uniform sampler2D texture_normal1;    // 法线贴图
uniform sampler2D texture_reflect1;// 反射强度（0~1）

void main()
{
    // 采样材质贴图
    vec3 diffuseColor = texture(texture_diffuse1, TexCoords).rgb;
    float roughness = texture(texture_specular1, TexCoords).r;
    float reflectStrength = texture(texture_reflect1, TexCoords).r;

    // 法线贴图扰动（简化处理，无 TBN）
    vec3 tangentNormal = texture(texture_normal1, TexCoords).rgb;
    tangentNormal = normalize(tangentNormal * 2.0 - 1.0); // 映射到 [-1, 1]
    vec3 perturbedNormal = normalize(tangentNormal);      // 暂时当作世界空间法线使用

    // 计算反射向量
    vec3 viewDir = normalize(Position - cameraPos);
    vec3 reflectDir = reflect(viewDir, perturbedNormal);

    // 环境反射颜色（假设你有一个天空盒）
    vec3 envColor = texture(texture_diffuse1, TexCoords).rgb; // 可替换为天空盒采样
    vec3 reflectionColor = textureLod(texture_diffuse1, TexCoords, roughness * 8.0).rgb;

    // 混合漫反射与反射
    vec3 finalColor = mix(diffuseColor, reflectionColor, reflectStrength * (1.0 - roughness));

    FragColor = vec4(finalColor, 1.0);
}