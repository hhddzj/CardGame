using UnityEngine;

/// <summary>
/// 玩家角色类，继承自Character，添加能量系统
/// </summary>
public class Player : Character
{
    /// <summary>
    /// 当前能量值，用于打出卡牌
    /// </summary>
    public int energy { get; private set; }

    /// <summary>
    /// 最大能量值
    /// </summary>
    public int MaxEnergy { get; private set; }

    /// <summary>
    /// 显示伤害/护盾弹出数字（玩家专用，显示在BattlePanel下）
    /// </summary>
    /// <param name="amount">数值</param>
    /// <param name="isBlock">是否为护盾</param>
    protected override void ShowDamagePopup(int amount, bool isBlock)
    {
        DamagePopup popup = DamagePopup.CreateForPlayer(transform);
        if (popup != null)
        {
            popup.ShowDamage(amount, isBlock);
        }
    }

    /// <summary>
    /// 初始化玩家属性
    /// </summary>
    /// <param name="maxHp">最大生命值</param>
    /// <param name="maxEne">最大能量值</param>
    public void InitPlayer(int maxHp, int maxEne)
    {
        maxHealth = maxHp;
        currentHealth = maxHp;
        MaxEnergy = maxEne;
        energy = MaxEnergy;
        block = 0;
        UpdateHealthUI();
        UpdateBlockUI();
    }

    /// <summary>
    /// 承受伤害，死亡时通知BattleManager检查战斗结束
    /// </summary>
    /// <param name="amount">伤害数值</param>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        
        if (!IsAlive() && BattleManager.Instance != null)
        {
            BattleManager.Instance.CheckBattleEnd();
        }
    }

    /// <summary>
    /// 消耗能量
    /// </summary>
    /// <param name="cost">消耗的能量值</param>
    public void SpendEnergy(int cost)
    {
        energy -= cost;
    }

    /// <summary>
    /// 重置能量为最大值
    /// </summary>
    public void ResetEnergy() => energy = MaxEnergy;
}