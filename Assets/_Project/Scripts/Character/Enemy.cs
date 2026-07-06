using UnityEngine;

/// <summary>
/// 敌人角色类，继承自Character，添加意图系统
/// </summary>
public class Enemy : Character
{
    /// <summary>
    /// 当前意图，包含攻击、防御、增益等类型
    /// </summary>
    public Intent currentIntent;

    /// <summary>
    /// 意图系统组件，负责显示意图图标和数值
    /// </summary>
    public IntentSystem intentSystem;

    /// <summary>
    /// 初始化意图系统组件
    /// </summary>
    private void Awake()
    {
        intentSystem = GetComponent<IntentSystem>();
        if (intentSystem == null)
        {
            intentSystem = gameObject.AddComponent<IntentSystem>();
        }
    }

    /// <summary>
    /// 决定敌人下一回合的意图
    /// 低血量时更倾向于防御，否则随机选择意图
    /// </summary>
    public void DecideIntent()
    {
        int r = Random.Range(0, 100);
        float healthPercent = (float)currentHealth / maxHealth;

        // 低血量时40%概率防御
        if (healthPercent < 0.3f && r < 40)
        {
            currentIntent = new Intent(IntentType.Defend, Random.Range(5, 10), "防御");
        }
        // 10%概率增益自身
        else if (r < 10)
        {
            currentIntent = new Intent(IntentType.Buff, Random.Range(2, 5), "力量提升");
        }
        // 默认攻击
        else
        {
            currentIntent = new Intent(IntentType.Attack, Random.Range(8, 15), "攻击");
        }

        // 更新意图UI
        if (intentSystem != null)
        {
            intentSystem.SetIntent(currentIntent);
        }
    }

    /// <summary>
    /// 执行当前意图
    /// </summary>
    public void ExecuteIntent()
    {
        if (currentIntent == null)
            return;

        switch (currentIntent.type)
        {
            case IntentType.Attack:
                AttackPlayer();
                break;
            case IntentType.Defend:
                Defend();
                break;
            case IntentType.Buff:
                BuffSelf();
                break;
            case IntentType.DeBuff:
                DebuffPlayer();
                break;
        }

        // 清除意图UI
        if (intentSystem != null)
        {
            intentSystem.ClearIntent();
        }
        currentIntent = null;
    }

    /// <summary>
    /// 攻击玩家
    /// </summary>
    private void AttackPlayer()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.player != null)
        {
            BattleManager.Instance.player.TakeDamage(currentIntent.value);
            Debug.Log($"敌人攻击玩家，造成 {currentIntent.value} 点伤害");
        }
    }

    /// <summary>
    /// 防御，获得护盾
    /// </summary>
    private void Defend()
    {
        AddBlock(currentIntent.value);
        Debug.Log($"敌人防御，获得 {currentIntent.value} 点护盾");
    }

    /// <summary>
    /// 增益自身（预留方法）
    /// </summary>
    private void BuffSelf()
    {
        Debug.Log($"敌人增益，力量提升 {currentIntent.value}");
    }

    /// <summary>
    /// 减益玩家（预留方法）
    /// </summary>
    private void DebuffPlayer()
    {
        Debug.Log($"敌人减益玩家");
    }

    /// <summary>
    /// 承受伤害，死亡时通知BattleManager移除敌人
    /// </summary>
    /// <param name="amount">伤害数值</param>
    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (currentHealth == 0 && BattleManager.Instance != null)
        {
            BattleManager.Instance.RemoveEnemy(this);
        }
    }
}