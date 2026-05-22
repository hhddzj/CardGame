using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPanel : UIPanel
{
    public GameObject titleText;
    public Button tcWindow;
    public TestWindow testWindow;
    public void Awake()
    {
        tcWindow.onClick.AddListener(SpawnWindow);
    }
    public void SpawnWindow()
    {
        TestWindow win = UIFrame.Instance.OpenDynamicWindow(testWindow) as TestWindow;
        if (win != null)
            win.Initialize("动态窗口 " + Random.Range(1, 100));
        

    }
    public override void OnEnter()
    {
        base.OnEnter();
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        }
    }
    public override void OnExit()
    {
        base.OnExit();
    }
    // 下面两个方法只有 UIWindow 用得到，Panel 可以留着备用
    public override void OnPause()
    {
        base.OnPause();
    }

    public override void OnResume()
    {
        base.OnResume();
    }
}
