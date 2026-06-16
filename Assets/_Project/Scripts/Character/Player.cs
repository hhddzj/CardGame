using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player : Character
{
    public int energy { get; private set; }
    public int MaxEnergy { get; private set; }

    // 删掉：public Player player; 内部嵌套自己毫无意义
    public List<Card> hand;
    public List<Card> discardPile;

    // 替换构造函数，用自定义初始化方法
    public void InitPlayer(int maxHp, int maxEne)
    {
        maxHealth = maxHp;
        currentHealth = maxHp;
        MaxEnergy = maxEne;
        energy = MaxEnergy;
        block = 0; // 父类格挡重置
        UpdateHealthUI();
    }

    public void SpendEnergy(int cost)
    {
        energy -= cost;
    }

    //回合开始回满费用
    public void ResetEnergy() => energy = MaxEnergy;

    public void CheckBattleEnd()
    {

    }
}