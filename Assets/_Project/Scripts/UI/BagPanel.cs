using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背包面板，显示玩家背包内容
/// </summary>
public class BagPanel : UIPanel
{
    /// <summary>
    /// 标题文本对象
    /// </summary>
    public GameObject titleText;

    /// <summary>
    /// 面板进入时调用，播放标题动画
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
    /// 面板退出时调用
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
    }

    /// <summary>
    /// 面板暂停时调用（仅 Window 层使用，Panel 可留作备用）
    /// </summary>
    public override void OnPause()
    {
        base.OnPause();
    }

    /// <summary>
    /// 面板恢复时调用（仅 Window 层使用，Panel 可留作备用）
    /// </summary>
    public override void OnResume()
    {
        base.OnResume();
    }
}
