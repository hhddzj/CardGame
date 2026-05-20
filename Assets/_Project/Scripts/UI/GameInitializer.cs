using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    public MainPanel mainPanelInScene;
    public MapPanel MapPanelInScene;
    public BagPanel BagPanelInScene;

    void Start()
    {
        // 实例化预制体
        // 注册这个已存在的面板
        UIFrame.Instance.RegisterPage(mainPanelInScene);
        UIFrame.Instance.RegisterPage(MapPanelInScene);
        UIFrame.Instance.RegisterPage(BagPanelInScene);
        // 游戏一开始打开主菜单（也可以是别的逻辑）
        UIFrame.Instance.OpenPanel("MainPanel");
    }
}
