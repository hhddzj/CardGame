using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    //public Intent currentIntent;

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
}