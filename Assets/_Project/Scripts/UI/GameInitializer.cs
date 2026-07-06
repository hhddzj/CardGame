using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏初始化器，负责在游戏启动时注册所有UI页面并打开主菜单
/// </summary>
public class GameInitializer : MonoBehaviour
{
    /// <summary>
    /// 场景中的主菜单面板
    /// </summary>
    public MainPanel mainPanelInScene;

    /// <summary>
    /// 场景中的地图面板
    /// </summary>
    public MapPanel MapPanelInScene;

    /// <summary>
    /// 场景中的背包面板
    /// </summary>
    public BagPanel BagPanelInScene;

    /// <summary>
    /// 场景中的设置窗口
    /// </summary>
    public SettingsWindow settingsWindowInScene;

    /// <summary>
    /// 场景中的战斗面板
    /// </summary>
    public BattlePanel BattlePanelInScene;

    /// <summary>
    /// 游戏启动时调用，注册所有UI页面并打开主菜单
    /// </summary>
    void Start()
    {
        UIFrame.Instance.RegisterPage(mainPanelInScene);
        UIFrame.Instance.RegisterPage(MapPanelInScene);
        UIFrame.Instance.RegisterPage(BagPanelInScene);
        UIFrame.Instance.RegisterPage(BattlePanelInScene);
        UIFrame.Instance.RegisterPage(settingsWindowInScene);

        UIFrame.Instance.OpenPanel("MainPanel");
    }
}
