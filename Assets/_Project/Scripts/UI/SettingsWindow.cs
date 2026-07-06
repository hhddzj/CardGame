using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 设置窗口，提供游戏设置和返回主菜单功能
/// </summary>
public class SettingsWindow : UIWindow
{
    /// <summary>
    /// 标题文本对象
    /// </summary>
    public GameObject titleText;

    /// <summary>
    /// 返回主菜单按钮
    /// </summary>
    public Button MainButton;

    /// <summary>
    /// 初始化按钮事件监听
    /// </summary>
    public override void Awake()
    {
        base.Awake();
        if (MainButton)
            MainButton.onClick.AddListener(OpenMainPanel);
    }

    /// <summary>
    /// 返回主菜单，关闭所有窗口
    /// </summary>
    public void OpenMainPanel()
    {
        UIFrame.Instance.OpenPanel("MainPanel");
        UIFrame.Instance.CloseAllWindow();
    }

    /// <summary>
    /// 窗口进入时调用，播放标题动画
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        }
    }

    /// <summary>
    /// 窗口退出时调用
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
    }

    /// <summary>
    /// 窗口暂停时调用（被新窗口遮挡时）
    /// </summary>
    public override void OnPause()
    {
        base.OnPause();
    }

    /// <summary>
    /// 窗口恢复时调用（遮挡窗口关闭后）
    /// </summary>
    public override void OnResume()
    {
        base.OnResume();
    }
}
