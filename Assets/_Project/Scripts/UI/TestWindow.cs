using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestWindow : UIWindow
{
    // Start is called before the first frame update
    public Text titleText;
    public TextMeshProUGUI DynamicWindowText;

    // 由外部设置唯一名称
    public void Initialize(string dynamicWindowText)
    {
        if (dynamicWindowText != null) DynamicWindowText.text = dynamicWindowText;
    }


    public override void OnExit()
    {
        base.OnExit();
    }
}
