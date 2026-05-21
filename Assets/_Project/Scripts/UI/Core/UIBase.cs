using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBase : MonoBehaviour
{
    public string pageName;// 页面唯一标识
    public bool isActive = false;//当前是否处于激活显示状态
    public Button closeButton;
    void Start()
    {
    }
    public virtual void OnClose()
    {
    }
    public virtual void OnEnter()
    {
        if (closeButton)
            closeButton.onClick.AddListener(OnClose);
        Debug.Log($"页面 {pageName} 被打开了");
    }   // 页面被打开时调用
    public virtual void OnPause()
    {
        Debug.Log($"页面 {pageName} 被暂停了");
    }   // 页面被新页面遮挡时调用（仅 Window 层）
    public virtual void OnResume()
    {
        Debug.Log($"页面 {pageName} 被恢复了");
    }  // 遮挡页关闭后恢复时调用（仅 Window 层）
    public virtual void OnExit()
    {
        Debug.Log($"页面 {pageName} 被关闭了");
    }    // 页面被关闭时调用
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class UIPanel : UIBase {
    public override void OnClose()
    {
        UIFrame.Instance.ClosePanel();
    }
}
public class UIWindow : UIBase {
    public override void OnClose()
    {
        UIFrame.Instance.CloseWindow();
    }
}