using System.Collections;
using System.Collections.Generic;
using Assets._Project.Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 主菜单面板，提供游戏开始和设置入口
/// </summary>
public class MainPanel : UIPanel
{
    /// <summary>
    /// 标题文本对象
    /// </summary>
    public GameObject titleText;

    /// <summary>
    /// 设置按钮
    /// </summary>
    public Button settingsButton;

    /// <summary>
    /// 开始游戏按钮
    /// </summary>
    public Button playgameButton;

    /// <summary>
    /// 初始化按钮事件监听
    /// </summary>
    void Start()
    {
        if (settingsButton)
            settingsButton.onClick.AddListener(OnSettings);
        if (playgameButton)
            playgameButton.onClick.AddListener(PlayGame);
    }

    /// <summary>
    /// 开始游戏，生成地图并进入地图面板
    /// </summary>
    void PlayGame()
    {
        if (MapManager.Instance == null)
        {
            GameObject mapManagerObj = new GameObject("MapManager");
            mapManagerObj.AddComponent<MapManager>();
        }
        else
        {
            MapManager.Instance.ResetMap();
        }
        MapManager.Instance.GenerateMap();
        UIFrame.Instance.OpenPanel("MapPanel");
        CardManager.Instance.InitGame();
    }

    /// <summary>
    /// 打开设置窗口
    /// </summary>
    void OnSettings()
    {
        UIFrame.Instance.OpenWindow("SettingsWindow");
    }

    /// <summary>
    /// 面板关闭时调用，记录游戏退出日志
    /// </summary>
    public override void OnClose()
    {
        Debug.Log($"游戏退出");
    }

    /// <summary>
    /// 面板进入时调用，播放标题动画
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        }
    }

    /// <summary>
    /// 面板退出时调用
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
    }

    /// <summary>
    /// 面板暂停时调用（仅 Window 层使用，Panel 可留作备用）
    /// </summary>
    public override void OnPause()
    {
        base.OnPause();
    }

    /// <summary>
    /// 面板恢复时调用（仅 Window 层使用，Panel 可留作备用）
    /// </summary>
    public override void OnResume()
    {
        base.OnResume();
    }
}
