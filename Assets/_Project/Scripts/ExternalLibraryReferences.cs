using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 外部库函数参考文档
/// 本文件记录项目中使用的所有外部库函数，包含功能说明、参数说明和使用示例
/// 不影响项目本体运行，仅作为参考文档
/// </summary>
public static class ExternalLibraryReferences
{
    #region DG.Tweening（Dotween动画库）

    /// <summary>
    /// Transform.DOScale - 缩放动画
    /// 对Transform进行缩放动画，支持多种参数配置
    /// </summary>
    /// <param name="target">目标缩放值</param>
    /// <param name="duration">动画时长（秒）</param>
    /// <returns>Tween对象，可用于链式调用</returns>
    /// <example>
    /// titleText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
    /// </example>

    /// <summary>
    /// Transform.DOFade - 淡入淡出动画
    /// 对CanvasGroup或Renderer进行透明度动画
    /// </summary>
    /// <param name="targetAlpha">目标透明度（0-1）</param>
    /// <param name="duration">动画时长（秒）</param>
    /// <returns>Tween对象，可用于链式调用</returns>
    /// <example>
    /// canvasGroup.DOFade(0f, 0.3f);
    /// </example>

    /// <summary>
    /// Tween.SetEase - 设置缓动函数
    /// 为动画设置缓动曲线，控制动画速度变化
    /// </summary>
    /// <param name="ease">缓动类型，如Ease.OutBounce、Ease.Linear等</param>
    /// <returns>Tween对象，可继续链式调用</returns>
    /// <example>
    /// transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
    /// </example>

    /// <summary>
    /// DOTween.Sequence - 创建动画序列
    /// 组合多个动画按顺序执行
    /// </summary>
    /// <returns>Sequence对象，可添加多个动画</returns>
    /// <example>
    /// Sequence seq = DOTween.Sequence();
    /// seq.Append(transform.DOScale(1.5f, 0.2f));
    /// seq.Append(transform.DOScale(1f, 0.3f));
    /// </example>

    /// <summary>
    /// Ease - 缓动类型枚举
    /// 定义动画的速度曲线类型
    /// </summary>
    /// <remarks>
    /// 常用值：
    /// - Ease.Linear: 线性（匀速）
    /// - Ease.OutBounce: 弹跳效果（结束时弹跳）
    /// - Ease.InOutQuad: 二次方缓动（开始和结束都慢）
    /// - Ease.InBack: 后退效果（开始时向后退）
    /// </remarks>

    #endregion

    #region TMPro（TextMeshPro文本库）

    /// <summary>
    /// TextMeshProUGUI - 高级文本组件
    /// 替代Unity原生Text的高性能文本渲染组件
    /// </summary>
    /// <remarks>
    /// 主要属性：
    /// - text: 显示的文本内容
    /// - fontSize: 字体大小
    /// - color: 字体颜色
    /// - alignment: 文本对齐方式
    /// </remarks>
    /// <example>
    /// TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
    /// text.text = "Hello World";
    /// text.fontSize = 24;
    /// </example>

    /// <summary>
    /// TMP_InputField - 输入框组件
    /// 支持高级文本输入的输入框组件
    /// </summary>

    #endregion

    #region UnityEngine.InputSystem（新输入系统）

    /// <summary>
    /// PlayerInputActions - 输入动作配置类
    /// 由Input Actions资源文件自动生成的输入动作配置类
    /// </summary>
    /// <remarks>
    /// 使用步骤：
    /// 1. 创建Input Actions资源文件
    /// 2. 在Inspector中配置动作和绑定
    /// 3. 生成C#类（点击Input Actions资源的"Generate C# Class"按钮）
    /// 4. 在代码中实例化并使用
    /// </remarks>
    /// <example>
    /// private PlayerInputActions inputActions;
    /// private void Awake() => inputActions = new PlayerInputActions();
    /// private void OnEnable() => inputActions.Enable();
    /// private void OnDisable() => inputActions.Disable();
    /// </example>

    /// <summary>
    /// InputAction - 单个输入动作
    /// 代表一个输入动作（如跳跃、移动、取消等）
    /// </summary>
    /// <remarks>
    /// 常用方法：
    /// - Enable(): 启用动作
    /// - Disable(): 禁用动作
    /// - FindAction(string name): 根据名称查找动作
    /// - performed: 动作执行时的回调事件
    /// </remarks>

    /// <summary>
    /// InputAction.CallbackContext - 输入回调上下文
    /// 包含输入动作执行时的详细信息
    /// </summary>
    /// <remarks>
    /// 常用属性：
    /// - action: 关联的InputAction
    /// - phase: 动作阶段（Started, Performed, Canceled）
    /// - ReadValue<T>(): 读取输入值
    /// </remarks>

    #endregion

    #region Unity.VisualScripting（可视化脚本）

    /// <summary>
    /// Unity.VisualScripting 命名空间
    /// 用于可视化脚本系统，通常在Inspector中配置，代码中较少直接调用
    /// </summary>

    #endregion

    #region UnityEngine.UI（Unity内置UI）

    /// <summary>
    /// Button - 按钮组件
    /// 用于处理点击事件的UI组件
    /// </summary>
    /// <remarks>
    /// 常用属性和方法：
    /// - onClick: 点击事件列表
    /// - interactable: 是否可交互
    /// - AddListener(Action): 添加点击事件监听
    /// </remarks>
    /// <example>
    /// Button btn = GetComponent<Button>();
    /// btn.onClick.AddListener(OnButtonClicked);
    /// </example>

    /// <summary>
    /// ScrollRect - 滚动视图组件
    /// 用于创建可滚动的内容区域
    /// </summary>
    /// <remarks>
    /// 常用属性：
    /// - content: 滚动内容容器
    /// - viewport: 视口区域
    /// - normalizedPosition: 归一化滚动位置（0-1）
    /// </remarks>

    /// <summary>
    /// RectTransform - 矩形变换组件
    /// UI元素的变换组件，用于控制位置、大小、锚点等
    /// </summary>
    /// <remarks>
    /// 常用属性：
    /// - anchoredPosition: 锚点位置
    /// - sizeDelta: 相对于锚点的大小
    /// - pivot: 轴心点（0-1）
    /// - anchorMin/anchorMax: 锚点范围（0-1）
    /// </remarks>

    /// <summary>
    /// Image - 图像组件
    /// 用于显示2D图像的UI组件
    /// </summary>
    /// <remarks>
    /// 常用属性：
    /// - color: 图像颜色（含透明度）
    /// - fillAmount: 填充比例（0-1，用于进度条等）
    /// - sprite: 精灵图像
    /// </remarks>

    /// <summary>
    /// Canvas - 画布组件
    /// 所有UI元素的根容器
    /// </summary>
    /// <remarks>
    /// 常用方法：
    /// - ForceUpdateCanvases(): 强制更新所有画布布局
    /// </remarks>

    #endregion

    #region UnityEngine.EventSystems（事件系统）

    /// <summary>
    /// PointerEventData - 指针事件数据
    /// 包含指针（鼠标/触摸）事件的详细信息
    /// </summary>
    /// <remarks>
    /// 常用属性：
    /// - position: 指针位置
    /// - delta: 指针移动增量
    /// - pressPosition: 按下位置
    /// </remarks>

    /// <summary>
    /// IPointerDownHandler - 指针按下接口
    /// 实现此接口处理指针按下事件
    /// </summary>
    /// <example>
    /// public void OnPointerDown(PointerEventData eventData)
    /// {
    ///     // 处理按下逻辑
    /// }
    /// </example>

    /// <summary>
    /// IBeginDragHandler - 拖拽开始接口
    /// 实现此接口处理拖拽开始事件
    /// </summary>
    /// <example>
    /// public void OnBeginDrag(PointerEventData eventData)
    /// {
    ///     // 记录初始位置
    /// }
    /// </example>

    /// <summary>
    /// IDragHandler - 拖拽中接口
    /// 实现此接口处理拖拽移动事件
    /// </summary>
    /// <example>
    /// public void OnDrag(PointerEventData eventData)
    /// {
    ///     // 更新位置
    /// }
    /// </example>

    /// <summary>
    /// IEndDragHandler - 拖拽结束接口
    /// 实现此接口处理拖拽结束事件
    /// </summary>
    /// <example>
    /// public void OnEndDrag(PointerEventData eventData)
    /// {
    ///     // 处理结束逻辑
    /// }
    /// </example>

    #endregion

    #region UnityEngine（Unity核心API）

    /// <summary>
    /// GameObject.Instantiate - 实例化对象
    /// 创建预制件或对象的副本
    /// </summary>
    /// <param name="original">原始对象/预制件</param>
    /// <param name="parent">父Transform（可选）</param>
    /// <returns>实例化的对象</returns>
    /// <example>
    /// Button btn = Instantiate(nodePrefab, mapContent);
    /// </example>

    /// <summary>
    /// GameObject.Destroy - 销毁对象
    /// 销毁指定对象，可延迟销毁
    /// </summary>
    /// <param name="obj">要销毁的对象</param>
    /// <example>
    /// Destroy(btn.gameObject);
    /// </example>

    /// <summary>
    /// MonoBehaviour.Invoke - 延迟调用
    /// 延迟指定时间后调用方法
    /// </summary>
    /// <param name="methodName">方法名称</param>
    /// <param name="time">延迟时间（秒）</param>
    /// <example>
    /// Invoke(nameof(SetScrollPosition), 0.1f);
    /// </example>

    /// <summary>
    /// MonoBehaviour.StartCoroutine - 启动协程
    /// 启动一个协程，用于异步操作
    /// </summary>
    /// <param name="routine">协程方法（返回IEnumerator）</param>
    /// <example>
    /// StartCoroutine(ExecuteTurn());
    /// </example>

    /// <summary>
    /// GameObject.FindObjectOfType - 查找对象
    /// 在场景中查找指定类型的对象
    /// </summary>
    /// <typeparam name="T">要查找的类型</typeparam>
    /// <returns>找到的对象，未找到返回null</returns>
    /// <example>
    /// BattleUIManager uiManager = FindObjectOfType<BattleUIManager>();
    /// </example>

    /// <summary>
    /// GameObject.GetComponent - 获取组件
    /// 获取对象上指定类型的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>找到的组件，未找到返回null</returns>
    /// <example>
    /// RectTransform rect = btn.GetComponent<RectTransform>();
    /// </example>

    /// <summary>
    /// GameObject.GetComponentInChildren - 获取子对象组件
    /// 在对象及其子对象中查找指定类型的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>找到的组件，未找到返回null</returns>
    /// <example>
    /// TextMeshProUGUI text = btn.GetComponentInChildren<TextMeshProUGUI>();
    /// </example>

    /// <summary>
    /// DontDestroyOnLoad - 场景切换时不销毁
    /// 使对象在场景切换时保持存在
    /// </summary>
    /// <param name="target">要保持的对象</param>
    /// <example>
    /// DontDestroyOnLoad(gameObject);
    /// </example>

    /// <summary>
    /// Debug.Log - 日志输出
    /// 在控制台输出日志信息
    /// </summary>
    /// <param name="message">日志消息</param>
    /// <example>
    /// Debug.Log("Page opened");
    /// </example>

    /// <summary>
    /// Debug.LogError - 错误日志输出
    /// 在控制台输出错误信息（红色）
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <example>
    /// Debug.LogError("Missing reference!");
    /// </example>

    /// <summary>
    /// Debug.LogWarning - 警告日志输出
    /// 在控制台输出警告信息（黄色）
    /// </summary>
    /// <param name="message">警告消息</param>
    /// <example>
    /// Debug.LogWarning("Action not found");
    /// </example>

    #endregion
}