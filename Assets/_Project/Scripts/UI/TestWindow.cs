using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestWindow : UIWindow
{
    // Start is called before the first frame update
    public Text titleText;
    public Button closeButton;
    private string myPageName;

    // 由外部设置唯一名称
    public void Initialize(string uniqueName)
    {
        myPageName = uniqueName;
        pageName = uniqueName;
        if (titleText) titleText.text = uniqueName;
        if (closeButton) closeButton.onClick.AddListener(OnClose);
    }

    void OnClose()
    {
        UIFrame.Instance.CloseWindow();   // 从栈中弹出自身
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
