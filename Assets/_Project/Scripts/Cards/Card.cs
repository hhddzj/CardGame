using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "基础卡牌", menuName = "卡牌配置/抽象卡牌")]
public abstract class Card : ScriptableObject
{
    /// <summary>
    /// 卡牌名称
    /// </summary>
    public string cardName;

    /// <summary>
    /// 卡牌费用，打出卡牌需要消耗的能量值
    /// </summary>
    public int cost;

    /// <summary>
    /// 卡牌类型（攻击、技能、能力）
    /// </summary>
    public CardType type;

    /// <summary>
    /// 卡牌描述，显示在卡牌上的文字说明
    /// </summary>
    public string description;

    /// <summary>
    /// 执行卡牌效果的抽象方法，由子类实现具体逻辑
    /// </summary>
    /// <param name="source">施法者角色</param>
    /// <param name="target">目标角色</param>
    public abstract void Play(Character source, Character target);
}

/// <summary>
/// 卡牌类型枚举
/// </summary>
public enum CardType
{
    /// <summary>
    /// 攻击牌，对敌人造成伤害
    /// </summary>
    Attack,

    /// <summary>
    /// 技能牌，提供防御、治疗等效果
    /// </summary>
    Skill,

    /// <summary>
    /// 能力牌，提供持续性增益效果
    /// </summary>
    Power
}