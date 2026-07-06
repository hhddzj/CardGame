using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// UI输入处理器，负责处理键盘快捷键与UI面板/窗口的映射
/// </summary>
public class UIInputHandler : MonoBehaviour
{
    /// <summary>
    /// UI动作绑定类，定义输入动作与UI页面的映射关系
    /// </summary>
    [System.Serializable]
    public class UIActionBinding
    {
        /// <summary>
        /// 与 PlayerInputActions 中的 Action 名一致
        /// </summary>
        public string actionName;

        /// <summary>
        /// 要打开的面板名称（或自定义命令）
        /// </summary>
        public string pageName;
    }

    /// <summary>
    /// 在 Inspector 中配置的输入动作与UI页面的映射数组
    /// </summary>
    [SerializeField] private UIActionBinding[] bindings;

    /// <summary>
    /// 输入动作配置实例
    /// </summary>
    private PlayerInputActions inputActions;

    /// <summary>
    /// 初始化输入动作配置
    /// </summary>
    private void Awake() => inputActions = new PlayerInputActions();

    /// <summary>
    /// 启用时订阅所有配置的输入动作
    /// </summary>
    private void OnEnable()
    {
        inputActions.Enable();

        foreach (var binding in bindings)
            SubscribeAction(binding.actionName);
    }

    /// <summary>
    /// 禁用时取消订阅所有输入动作
    /// </summary>
    private void OnDisable()
    {
        foreach (var binding in bindings)
            UnsubscribeAction(binding.actionName);

        inputActions.Disable();
    }

    /// <summary>
    /// 订阅指定名称的输入动作
    /// </summary>
    /// <param name="actionName">动作名称</param>
    private void SubscribeAction(string actionName)
    {
        var action = inputActions.FindAction(actionName);
        if (action != null)
            action.performed += OnActionPerformed;
        else
            Debug.LogWarning($"Action '{actionName}' not found in PlayerInputActions");
    }

    /// <summary>
    /// 取消订阅指定名称的输入动作
    /// </summary>
    /// <param name="actionName">动作名称</param>
    private void UnsubscribeAction(string actionName)
    {
        var action = inputActions.FindAction(actionName);
        if (action != null)
            action.performed -= OnActionPerformed;
    }

    /// <summary>
    /// 输入动作执行时的回调处理
    /// </summary>
    /// <param name="ctx">输入动作回调上下文</param>
    private void OnActionPerformed(InputAction.CallbackContext ctx)
    {
        foreach (var binding in bindings)
        {
            if (binding.actionName == ctx.action.name)
            {
                ExecuteBinding(binding);
                break;
            }
        }
    }

    /// <summary>
    /// 执行绑定的UI操作
    /// </summary>
    /// <param name="binding">动作绑定</param>
    private void ExecuteBinding(UIActionBinding binding)
    {
        if (binding.actionName == "Cancel")
        {
            if (UIFrame.Instance.CurrentWindow != null && UIFrame.Instance.CurrentWindow.pageName == binding.pageName)
                UIFrame.Instance.CloseWindow();
            else
                UIFrame.Instance.OpenWindow(binding.pageName);
        }
        else
        {
            if (UIFrame.Instance.CurrentPanel != null
            && binding.pageName == UIFrame.Instance.CurrentPanel.pageName)
            {
                UIFrame.Instance.ClosePanel();
            }
            else
            {
                UIFrame.Instance.OpenPanel(binding.pageName);
            }
        }
    }
}