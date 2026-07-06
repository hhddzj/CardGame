using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

/// <summary>
/// 卡牌UI组件，负责卡牌的显示和点击交互
/// </summary>
public class CardUI : MonoBehaviour
{
    /// <summary>
    /// 卡牌数据对象
    /// </summary>
    public Card cardData;

    /// <summary>
    /// 卡牌名称
    /// </summary>
    public string cardName;

    /// <summary>
    /// 卡牌费用
    /// </summary>
    public int cost;

    /// <summary>
    /// 卡牌描述
    /// </summary>
    public string description;

    /// <summary>
    /// 点击卡牌时执行，播放卡牌效果
    /// 默认选择第一个敌人作为目标（实际应由玩家拖拽选择目标）
    /// </summary>
    public void OnClick()
    {
        BattleManager.Instance.PlayCard(cardData, BattleManager.Instance.enemies[0]);
    }
}