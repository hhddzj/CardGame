using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : Card
{
    public string cardName="攻击";
    public int cost=1;
    public CardType type=CardType.Attack; // Attack, Skill, Power
    public string description="6伤害的卡牌";
    public override void Play(Character source, Character target)
    {
        int damage = 6; // 基础伤害，可以加上 source 的力量加成
        target.TakeDamage(damage);
    }
}
