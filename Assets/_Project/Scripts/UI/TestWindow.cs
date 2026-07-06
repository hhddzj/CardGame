using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 测试窗口，用于动态显示测试内容
/// </summary>
public class TestWindow : UIWindow
{
    /// <summary>
    /// 标题文本组件
    /// </summary>
    public Text titleText;

    /// <summary>
    /// 动态窗口文本组件
    /// </summary>
    public TextMeshProUGUI DynamicWindowText;

    /// <summary>
    /// 初始化窗口，设置动态文本内容
    /// </summary>
    /// <param name="dynamicWindowText">要显示的动态文本</param>
    public void Initialize(string dynamicWindowText)
    {
        if (dynamicWindowText != null) DynamicWindowText.text = dynamicWindowText;
    }

    /// <summary>
    /// 窗口退出时调用
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
    }
}
