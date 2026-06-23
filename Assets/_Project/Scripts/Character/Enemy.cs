using UnityEngine;

public class Enemy : Character
{
    public Intent currentIntent;
    public IntentSystem intentSystem;

    private void Awake()
    {
        intentSystem = GetComponent<IntentSystem>();
        if (intentSystem == null)
        {
            intentSystem = gameObject.AddComponent<IntentSystem>();
        }
    }

    public void DecideIntent()
    {
        int r = Random.Range(0, 100);
        float healthPercent = (float)currentHealth / maxHealth;

        if (healthPercent < 0.3f && r < 40)
        {
            currentIntent = new Intent(IntentType.Defend, Random.Range(5, 10), "防御");
        }
        else if (r < 10)
        {
            currentIntent = new Intent(IntentType.Buff, Random.Range(2, 5), "力量提升");
        }
        else
        {
            currentIntent = new Intent(IntentType.Attack, Random.Range(8, 15), "攻击");
        }

        if (intentSystem != null)
        {
            intentSystem.SetIntent(currentIntent);
        }
    }

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

        if (intentSystem != null)
        {
            intentSystem.ClearIntent();
        }
        currentIntent = null;
    }

    private void AttackPlayer()
    {
        if (BattleManager.Instance != null && BattleManager.Instance.player != null)
        {
            BattleManager.Instance.player.TakeDamage(currentIntent.value);
            Debug.Log($"敌人攻击玩家，造成 {currentIntent.value} 点伤害");
        }
    }

    private void Defend()
    {
        AddBlock(currentIntent.value);
        Debug.Log($"敌人防御，获得 {currentIntent.value} 点护盾");
    }

    private void BuffSelf()
    {
        Debug.Log($"敌人增益，力量提升 {currentIntent.value}");
    }

    private void DebuffPlayer()
    {
        Debug.Log($"敌人减益玩家");
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);

        if (currentHealth == 0 && BattleManager.Instance != null)
        {
            BattleManager.Instance.RemoveEnemy(this);
        }
    }
}