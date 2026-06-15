using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
public class CardUI : MonoBehaviour
{
    public Card cardData;
    public string cardName;
    public int cost;
    public string description;

    // 效果执行
    public void OnClick()
    {
        // 假设我们选定敌人0为目标（实际应由玩家选择目标）
        BattleManager.Instance.PlayCard(cardData, BattleManager.Instance.enemies[0]);
    }

}