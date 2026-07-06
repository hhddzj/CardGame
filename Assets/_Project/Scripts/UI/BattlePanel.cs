using Assets._Project.Scripts.Managers;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 战斗面板，负责战斗场景的初始化和战斗结束处理
/// </summary>
public class BattlePanel : UIPanel
{
    /// <summary>
    /// 标题文本对象
    /// </summary>
    public GameObject titleText;

    /// <summary>
    /// 面板进入时调用，初始化战斗并播放标题动画
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("[BattlePanel] OnEnter - Starting new battle");
        
        if (titleText != null)
        {
            titleText.transform.localScale = Vector3.zero;
            titleText.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBounce);
        }

        CardManager.Instance.InitBattle();
        CardManager.Instance.DisCard(5);
        BattleUIManager.Instance.StartGame();

        BattleManager.Instance.OnBattleEnd += OnBattleEndHandler;
    }

    /// <summary>
    /// 战斗结束回调处理函数
    /// </summary>
    /// <param name="result">战斗结果（胜利/失败）</param>
    private void OnBattleEndHandler(BattleResult result)
    {
        BattleManager.Instance.OnBattleEnd -= OnBattleEndHandler;

        if (result == BattleResult.Victory)
        {
            HandleVictory();
        }
        else
        {
            HandleDefeat();
        }
    }

    /// <summary>
    /// 处理战斗胜利逻辑
    /// </summary>
    private void HandleVictory()
    {
        Debug.Log("战斗胜利");
        if (BattleManager.Instance.battleDifficulty == 3)
        {
            Debug.Log("Boss战胜利！返回主菜单");
            MapManager.Instance.ResetMap();
            UIFrame.Instance.OpenPanel("MainPanel");
        }
        else
        {
            UIFrame.Instance.OpenPanel("MapPanel");
        }
    }

    /// <summary>
    /// 处理战斗失败逻辑，重置地图并返回主菜单
    /// </summary>
    private void HandleDefeat()
    {
        Debug.Log("战斗失败");
        if (MapManager.Instance != null)
        {
            MapManager.Instance.ResetMap();
            Debug.Log("地图已重置");
        }
        UIFrame.Instance.OpenPanel("MainPanel");
    }

    /// <summary>
    /// 面板退出时调用，取消战斗结束事件订阅
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        BattleManager.Instance.OnBattleEnd -= OnBattleEndHandler;
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
