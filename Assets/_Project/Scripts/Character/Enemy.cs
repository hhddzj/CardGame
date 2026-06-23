using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Enemy : Character
{
    //public Intent currentIntent;
    public Enemy(int maxHp, int maxEne)
    {
        maxHealth = maxHp;
        currentHealth = maxHp;
    }
    public void DecideIntent()
    {
        // 简单规则：血量低时可能防御，否则攻击
        int r = Random.Range(0, 100);
        /*if (currentHealth < maxHealth * 0.3f && r < 40)
            currentIntent = new Intent(IntentType.Defend, 8);
        else
            currentIntent = new Intent(IntentType.Attack, 12);
        */
        // 更新UI显示意图图标和数值
    }
    public override void TakeDamage(int amount)
    {
        if (amount <= 0) return;

        // 护盾减伤逻辑（优化版）
        if (block > 0)
        {
            if (amount >= block)
            {
                amount -= block;
                block = 0;
            }
            else
            {
                block -= amount;
                amount = 0;
            }
        }

        // 扣除血量
        currentHealth -= amount;
        if (currentHealth < 0)
            currentHealth = 0;
        if(currentHealth==0)
        {
            BattleManager.Instance.enemies.Remove(this);
            Destroy(gameObject);
        }
        // 更新UI（关键！）
        UpdateHealthUI();
    }
}