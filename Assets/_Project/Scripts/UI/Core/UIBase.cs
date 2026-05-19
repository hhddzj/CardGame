using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public string pageName;// 页面唯一标识
    public bool isActive = false;//当前是否处于激活显示状态
    public virtual void OnEnter() { }   // 页面被打开时调用
    public virtual void OnPause() { }   // 页面被新页面遮挡时调用（仅 Window 层）
    public virtual void OnResume() { }  // 遮挡页关闭后恢复时调用（仅 Window 层）
    public virtual void OnExit() { }    // 页面被关闭时调用
}

public class UIPanel : UIBase { }
public class UIWindow : UIBase { }