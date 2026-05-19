using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    public MainPanel mainPanelPrefab;

    void Start()
    {
        // 实例化预制体
        MainPanel mainPanelInstance = Instantiate(mainPanelPrefab);
        // 注册到 UIFrame 框架中
        UIFrame.Instance.RegisterPage(mainPanelInstance);

        // 游戏一开始，直接打开主菜单
        UIFrame.Instance.OpenPanel("MainPanel");
    }
}
