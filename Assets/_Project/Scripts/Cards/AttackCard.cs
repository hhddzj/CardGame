using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "打击卡", menuName = "卡牌配置/攻击卡")]
public class AttackCard : Card
{
    public int damage;

    public override void Play(Character source, Character target)
    {
        target.TakeDamage(damage);
    }
}