using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class MainPanel : UIPanel
{
    public GameObject titleText;
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log($"页面 {pageName} 被打开了"); 
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        }
    }
    public override void OnExit()
    {
        base.OnExit();
        Debug.Log($"页面 {pageName} 被关闭了");
    }
    // 下面两个方法只有 UIWindow 用得到，Panel 可以留着备用
    public override void OnPause()
    {
        base.OnPause();
        Debug.Log($"页面 {pageName} 被暂停了");
    }

    public override void OnResume()
    {
        base.OnResume();
        Debug.Log($"页面 {pageName} 被恢复了");
    }
}
