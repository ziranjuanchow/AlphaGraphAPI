flowchart TD

    A[开始渲染循环] --> B[计算时间差deltaTime]
    B --> C[处理用户输入processInput]
    C --> D[清除颜色/深度/模板缓冲区`<br/>`(glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT | GL_STENCIL_BUFFER_BIT))]
    D --> E[启用模板测试`<br/>`(glEnable(GL_STENCIL_TEST))]  // 新增：启用模板测试（默认关闭）

    E --> F[设置着色器uniforms`<br/>`(如时间、全局参数等)]
    F --> G[使用普通着色器shaderNormal]  // 明确着色器名称，避免歧义
    G --> H[设置视图和投影矩阵到shaderNormal]
    H --> I[第一阶段: 渲染地板（不影响模板）]
    I --> J[禁用模板写入`<br/>`(glStencilMask(0x00))]  // 地板不写入模板
    J --> K[绑定平面VAO和地板纹理]
    K --> L[渲染平面`<br/>`(glDrawArrays(GL_TRIANGLES, 0, 6))]

    L --> M[第二阶段: 渲染立方体并写入模板]
    M --> N[设置模板函数: 总是通过并写入模板`<br/>`(glStencilFunc(GL_ALWAYS, 1, 0xFF); glStencilOp(GL_KEEP, GL_KEEP, GL_REPLACE))]  // 补充模板操作符
    N --> O[启用模板写入`<br/>`(glStencilMask(0xFF))]
    O --> P[绑定立方体VAO和立方体纹理]
    P --> Q[设置第一个立方体模型矩阵→渲染`<br/>`(glDrawArrays)]
    Q --> R[设置第二个立方体模型矩阵→渲染`<br/>`(glDrawArrays)]

    R --> S[第三阶段: 渲染立方体轮廓（模板筛选）]
    S --> T[设置模板函数: 仅模板值≠1时通过`<br/>`(glStencilFunc(GL_NOTEQUAL, 1, 0xFF); glStencilOp(GL_KEEP, GL_KEEP, GL_KEEP))]  // 补充模板操作符（不更新模板）
    T --> U[禁用模板写入`<br/>`(glStencilMask(0x00))]
    U --> V[禁用深度测试`<br/>`(glDisable(GL_DEPTH_TEST))]  // 避免轮廓被立方体遮挡
    V --> W[切换到单色着色器shaderSingleColor]  // 轮廓用纯色（如红色）
    W --> X[设置视图和投影矩阵到shaderSingleColor]  // 补充：单色着色器也需矩阵
    X --> Y[设置立方体缩放因子1.1（放大产生轮廓）]
    Y --> Z[渲染放大版第一个立方体`<br/>`(glDrawArrays)]
    Z --> AA[渲染放大版第二个立方体`<br/>`(glDrawArrays)]

    AA --> AB[恢复OpenGL状态]
    AB --> AC[切换回普通着色器shaderNormal]  // 新增：恢复着色器，避免后续出错
    AC --> AD[启用深度测试`<br/>`(glEnable(GL_DEPTH_TEST))]
    AD --> AE[重置模板函数为默认值`<br/>`(glStencilFunc(GL_ALWAYS, 0, 0xFF))]  // 先重置函数
    AE --> AF[恢复模板写入`<br/>`(glStencilMask(0xFF))]  // 再恢复写入权限
    AF --> AG[禁用模板测试（可选，避免影响其他渲染）`<br/>`(glDisable(GL_STENCIL_TEST))]  // 新增：按需禁用，减少状态干扰

    AG --> AH[交换前后缓冲区+处理事件`<br/>`(glfwSwapBuffers + glfwPollEvents)]
    AH --> AI{窗口是否应该关闭?`<br/>`(glfwWindowShouldClose)}
    AI -->|否| B
    AI -->|是| AJ[结束程序]
