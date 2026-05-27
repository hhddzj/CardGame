using UnityEngine;
using UnityEngine.InputSystem;

public class UIInputHandler : MonoBehaviour
{
    [System.Serializable]
    public class UIActionBinding
    {
        public string actionName;   // 与 PlayerInputActions 中的 Action 名一致
        public string pageName;     // 要打开的面板名称（或自定义命令）
    }

    [SerializeField] private UIActionBinding[] bindings; // 在 Inspector 中配置映射

    private PlayerInputActions inputActions;

    private void Awake() => inputActions = new PlayerInputActions();

    private void OnEnable()
    {
        inputActions.Enable();

        // 动态订阅所有配置的 Action
        foreach (var binding in bindings)
            SubscribeAction(binding.actionName);
    }

    private void OnDisable()
    {
        // 动态取消订阅
        foreach (var binding in bindings)
            UnsubscribeAction(binding.actionName);

        inputActions.Disable();
    }

    private void SubscribeAction(string actionName)
    {
        var action = inputActions.FindAction(actionName);
        if (action != null)
            action.performed += OnActionPerformed;
        else
            Debug.LogWarning($"Action '{actionName}' not found in PlayerInputActions");
    }

    private void UnsubscribeAction(string actionName)
    {
        var action = inputActions.FindAction(actionName);
        if (action != null)
            action.performed -= OnActionPerformed;
    }

    private void OnActionPerformed(InputAction.CallbackContext ctx)
    {
        // 根据触发的 action 名字，找到对应的 binding
        foreach (var binding in bindings)
        {
            if (binding.actionName == ctx.action.name)
            {
                ExecuteBinding(binding);
                break;
            }
        }
    }

    private void ExecuteBinding(UIActionBinding binding)
    {
        // 特殊处理：如果是 Cancel（ESC），保持原来的切换逻辑
        if (binding.actionName == "Cancel")
        {
            if (UIFrame.Instance.CurrentWindow!=null&& UIFrame.Instance.CurrentWindow.pageName == binding.pageName)
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
                // 其他快捷键：直接切换面板（如果已经是当前面板则关闭？根据需要调整）
                
            else
            {
                UIFrame.Instance.OpenPanel(binding.pageName);
            }
        }
    }
}