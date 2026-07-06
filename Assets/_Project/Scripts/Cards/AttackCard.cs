using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "打击卡", menuName = "卡牌配置/攻击卡")]
public class AttackCard : Card
{
    /// <summary>
    /// 攻击力，对目标造成的伤害值
    /// </summary>
    public int damage;

    /// <summary>
    /// 执行攻击卡牌效果，对目标造成damage点伤害
    /// </summary>
    /// <param name="source">施法者角色</param>
    /// <param name="target">目标角色</param>
    public override void Play(Character source, Character target)
    {
        target.TakeDamage(damage);
    }
}