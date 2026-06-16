using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
[CreateAssetMenu(fileName = "基础卡牌", menuName = "卡牌配置/抽象卡牌")]
public abstract class Card : ScriptableObject
{
    public string cardName;
    public int cost;
    public CardType type; // Attack, Skill, Power
    public string description;

    // 效果执行
    public abstract void Play(Character source, Character target);

}
public enum CardType
{
    Attack, Skill, Power
}