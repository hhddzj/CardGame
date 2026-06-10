using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player : Character
{
    public int Energy;
    public int MaxEnergy;
    private Player _player;
    public Player player => _player;
    public List<Card> hand;
    public List<Card> discardPile;
    public Player(int maxHp, int maxEne)
    {
        maxHealth = maxHp;
        currentHealth = maxHp;
        MaxEnergy = maxEne;
        Energy = MaxEnergy;
    }
    //回合开始回满费用
    public void ResetEnergy() => Energy = MaxEnergy;
    public void CheckBattleEnd()
    {

    }
    public void UseCard(Card card, Character target)
    {
        if (player.Energy < card.cost) return;

        player.Energy -= card.cost;
        card.Play(player, target);      // 执行卡牌效果
        hand.Remove(card);
        discardPile.Add(card);

        // 检查战斗结束
        CheckBattleEnd();
    }
}