using Assets._Project.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("角色引用")]
    public Player player;
    public List<Enemy> enemies;

    [Header("卡牌系统")]
    public BattleState currentState;

    [Header("怪物生成")]
    public GameObject enemyPrefab;
    public Transform enemySpawnPoint;
    public EnemyLayoutHelper enemyLayoutHelper;

    [Header("难度配置")]
    public int maxEnemyCount = 3;
    public int baseEnemyHealth = 40;
    public int healthIncreasePerEnemy = 10;

    public Button button;
    public static BattleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeState(BattleState newState)
    {
        currentState = newState;
        Debug.Log($"战斗状态变更: {currentState}");
    }

    public void InitBattle()
    {
        Debug.Log("初始化战斗");
        ChangeState(BattleState.PlayerAction);

        GenerateEnemies();

        if (player != null)
        {
            player.InitPlayer(30, 3);
            player.ResetEnergy();
        }

        CardManager.Instance.DisCard(5);
        BattleUIManager.Instance.RefreshAllUI();
    }

    private void GenerateEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned!");
            return;
        }

        int enemyCount = Random.Range(1, maxEnemyCount + 1);
        Debug.Log($"生成 {enemyCount} 个敌人");

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, enemySpawnPoint);
            Enemy enemy = enemyObj.GetComponent<Enemy>();

            if (enemy != null)
            {
                int health = baseEnemyHealth + i * healthIncreasePerEnemy;
                enemy.maxHealth = health;
                enemy.currentHealth = health;
                enemy.name = $"Enemy_{i + 1}";
                enemy.DecideIntent();
                enemies.Add(enemy);

                if (enemyLayoutHelper != null)
                {
                    enemyLayoutHelper.AddEnemy(enemy.transform);
                }
            }
            else
            {
                Debug.LogError("Enemy prefab does not have Enemy component!");
                Destroy(enemyObj);
            }
        }
    }

    public void PlayCard(Card card, Character target)
    {
        Debug.Log("PlayCard 被调用，状态=" + currentState);
        if (currentState != BattleState.PlayerAction) return;
        if (player.energy < card.cost) return;

        player.SpendEnergy(card.cost);
        card.Play(player, target);
        CardManager.Instance.DiscardCard(card);
        Debug.Log($"{card.cardName} 对 {target.name} 生效");

        CheckBattleEnd();
        BattleUIManager.Instance.RefreshAllUI();

        foreach (Enemy enemy in enemies)
        {
            enemy.DecideIntent();
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);

            if (enemyLayoutHelper != null)
            {
                enemyLayoutHelper.RemoveEnemy(enemy.transform);
            }

            Destroy(enemy.gameObject);
        }
    }

    void CheckBattleEnd()
    {
        if (enemies.Count == 0)
        {
            Debug.Log("游戏胜利");
            ChangeState(BattleState.Victory);
        }
        else if (!player.IsAlive())
        {
            Debug.Log("游戏失败");
            ChangeState(BattleState.Defeat);
        }
    }

    public void NextTurn()
    {
        StartCoroutine(ExecuteTurnSequence());
    }

    private IEnumerator ExecuteTurnSequence()
    {
        yield return StartCoroutine(PlayerTurnEndCoroutine());
        yield return StartCoroutine(EnemyTurnCoroutine());
        yield return StartCoroutine(PlayerTurnStartCoroutine());
    }

    private IEnumerator PlayerTurnEndCoroutine()
    {
        ChangeState(BattleState.PlayerTurnEnd);
        CardManager.Instance.TurnEnd();
        player.ClearBlock();
        Debug.Log("结算回合结束效果");
        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator EnemyTurnCoroutine()
    {
        ChangeState(BattleState.EnemyTurnStart);
        Debug.Log("敌人执行意图");

        foreach (Enemy enemy in enemies)
        {
            if (!enemy.IsAlive()) continue;

            yield return new WaitForSeconds(0.8f);
            enemy.ExecuteIntent();
            BattleUIManager.Instance.RefreshAllUI();
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator PlayerTurnStartCoroutine()
    {
        ChangeState(BattleState.PlayerTurnStart);
        Debug.Log("回能量、抽牌、重置格挡");

        player.ResetEnergy();
        CardManager.Instance.TurnStart();
        player.ClearBlock();

        yield return new WaitForSeconds(0.3f);

        ChangeState(BattleState.PlayerAction);
        Debug.Log("玩家出牌");
        BattleUIManager.Instance.RefreshAllUI();

        foreach (Enemy enemy in enemies)
        {
            enemy.DecideIntent();
        }
    }
}
