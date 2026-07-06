using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI页面基类，定义页面的基本生命周期和行为
/// </summary>
public class UIBase : MonoBehaviour
{
    /// <summary>
    /// 页面唯一标识名称
    /// </summary>
    public string pageName;

    /// <summary>
    /// 当前是否处于激活显示状态
    /// </summary>
    public bool isActive = false;

    /// <summary>
    /// 关闭按钮
    /// </summary>
    public Button closeButton;

    /// <summary>
    /// 初始化，绑定关闭按钮事件
    /// </summary>
    public virtual void Awake()
    {
        if (closeButton)
            closeButton.onClick.AddListener(OnClose);
    }

    void Start()
    {
    }

    /// <summary>
    /// 页面关闭时调用，子类可重写
    /// </summary>
    public virtual void OnClose()
    {
    }

    /// <summary>
    /// 页面被打开时调用
    /// </summary>
    public virtual void OnEnter()
    {
        Debug.Log($"页面 {pageName} 被打开了");
    }

    /// <summary>
    /// 页面被新页面遮挡时调用（仅 Window 层）
    /// </summary>
    public virtual void OnPause()
    {
        Debug.Log($"页面 {pageName} 被暂停了");
    }

    /// <summary>
    /// 遮挡页关闭后恢复时调用（仅 Window 层）
    /// </summary>
    public virtual void OnResume()
    {
        Debug.Log($"页面 {pageName} 被恢复了");
    }

    /// <summary>
    /// 页面被关闭时调用
    /// </summary>
    public virtual void OnExit()
    {
        Debug.Log($"页面 {pageName} 被关闭了");
    }

    void Update()
    {
        
    }
}

/// <summary>
/// UI面板类，继承自UIBase，用于全屏页面
/// </summary>
public class UIPanel : UIBase
{
    /// <summary>
    /// 关闭面板，调用UIFrame的ClosePanel方法
    /// </summary>
    public override void OnClose()
    {
        UIFrame.Instance.ClosePanel();
    }
}

/// <summary>
/// UI窗口类，继承自UIBase，用于弹出窗口
/// </summary>
public class UIWindow : UIBase
{
    /// <summary>
    /// 是否为动态窗口（运行时创建，关闭时销毁）
    /// </summary>
    public bool isDynamicWindow = false;

    /// <summary>
    /// 关闭窗口，调用UIFrame的CloseSpecificWindow方法
    /// </summary>
    public override void OnClose()
    {
        UIFrame.Instance.CloseSpecificWindow(this);
    }
}