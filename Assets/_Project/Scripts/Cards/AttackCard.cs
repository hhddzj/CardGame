using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCard : Card
{
    public string cardName="攻击";
    public int cost=1;
    public CardType type=CardType.Attack; // Attack, Skill, Power
    public string description="一攻击的卡牌";
    public override void Play(Character source, Character target)
    {
        source = BattleManager.Instance.player;
        target = BattleManager.Instance.monster;
        source.TakeDamage(10);
    }
}
