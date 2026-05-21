using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : UIPanel
{
    public GameObject titleText;
    public Button settingsButton;
    public Button closeButton;
    void Start()
    {
        if (settingsButton)
            settingsButton.onClick.AddListener(OnSettings);
    }
    void OnSettings()
    {
        UIFrame.Instance.OpenWindow("SettingsWindow");
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
