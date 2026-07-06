using Assets._Project.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗结果枚举
/// </summary>
public enum BattleResult
{
    /// <summary>
    /// 胜利
    /// </summary>
    Victory,

    /// <summary>
    /// 失败
    /// </summary>
    Defeat
}

/// <summary>
/// 战斗管理器，负责管理战斗流程、回合管理、战斗状态控制等核心战斗业务逻辑
/// </summary>
public class BattleManager : MonoBehaviour
{
    [Header("角色引用")]

    /// <summary>
    /// 玩家角色
    /// </summary>
    public Player player;

    /// <summary>
    /// 敌人列表
    /// </summary>
    public List<Enemy> enemies;

    [Header("卡牌系统")]

    /// <summary>
    /// 当前战斗状态
    /// </summary>
    public BattleState currentState;

    [Header("怪物生成")]

    /// <summary>
    /// 敌人预制体
    /// </summary>
    public GameObject enemyPrefab;

    /// <summary>
    /// 敌人生成位置
    /// </summary>
    public Transform enemySpawnPoint;

    /// <summary>
    /// 敌人布局辅助器
    /// </summary>
    public EnemyLayoutHelper enemyLayoutHelper;

    [Header("难度配置")]

    /// <summary>
    /// 最大敌人数量
    /// </summary>
    public int maxEnemyCount = 3;

    /// <summary>
    /// 基础敌人生命值
    /// </summary>
    public int baseEnemyHealth = 40;

    /// <summary>
    /// 每增加一个敌人增加的生命值
    /// </summary>
    public int healthIncreasePerEnemy = 10;

    /// <summary>
    /// 战斗难度（1=普通，2=精英，3=Boss）
    /// </summary>
    public int battleDifficulty = 1;

    /// <summary>
    /// 测试按钮
    /// </summary>
    public Button button;

    /// <summary>
    /// 单例实例
    /// </summary>
    public static BattleManager Instance { get; private set; }

    /// <summary>
    /// 战斗结束事件，通知UI层战斗结果
    /// </summary>
    public event System.Action<BattleResult> OnBattleEnd;

    /// <summary>
    /// 初始化单例
    /// </summary>
    private void Awake()
    {
        if (Instance) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 变更战斗状态
    /// </summary>
    /// <param name="newState">新的战斗状态</param>
    public void ChangeState(BattleState newState)
    {
        currentState = newState;
        Debug.Log($"战斗状态变更: {currentState}");
    }

    /// <summary>
    /// 设置战斗难度
    /// </summary>
    /// <param name="difficulty">难度值（1-3）</param>
    public void SetBattleDifficulty(int difficulty)
    {
        battleDifficulty = Mathf.Clamp(difficulty, 1, 3);
        Debug.Log($"战斗难度设置为: {battleDifficulty}");
    }

    /// <summary>
    /// 初始化战斗，生成敌人并重置玩家状态
    /// </summary>
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

        BattleUIManager.Instance.RefreshAllUI();
    }

    /// <summary>
    /// 生成敌人，根据难度调整敌人数量和生命值
    /// </summary>
    private void GenerateEnemies()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab is not assigned!");
            return;
        }

        // Boss战只生成1个敌人
        int enemyCount = battleDifficulty == 3 ? 1 : Random.Range(1, maxEnemyCount + 1);
        int difficultyMultiplier = battleDifficulty;
        Debug.Log($"生成 {enemyCount} 个敌人，难度系数: {difficultyMultiplier}");

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyObj = Instantiate(enemyPrefab, enemySpawnPoint);
            Enemy enemy = enemyObj.GetComponent<Enemy>();

            if (enemy != null)
            {
                // 根据难度和序号计算生命值
                int health = (baseEnemyHealth + i * healthIncreasePerEnemy) * difficultyMultiplier;
                enemy.maxHealth = health;
                enemy.currentHealth = health;
                enemy.name = battleDifficulty == 3 ? "Boss" : $"Enemy_{i + 1}";
                enemy.DecideIntent();
                enemies.Add(enemy);

                // 添加到布局辅助器
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

    /// <summary>
    /// 播放卡牌，消耗能量并执行卡牌效果
    /// </summary>
    /// <param name="card">要播放的卡牌</param>
    /// <param name="target">目标角色</param>
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

        // 敌人决定下一回合意图
        foreach (Enemy enemy in enemies)
        {
            enemy.DecideIntent();
        }
    }

    /// <summary>
    /// 移除敌人，从列表和布局中移除并销毁对象
    /// </summary>
    /// <param name="enemy">要移除的敌人</param>
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

    /// <summary>
    /// 检查战斗是否结束，所有敌人死亡则胜利，玩家死亡则失败
    /// 可外部调用以触发战斗结束检查
    /// </summary>
    public void CheckBattleEnd()
    {
        if (enemies.Count == 0)
        {
            Debug.Log("游戏胜利");
            ChangeState(BattleState.Victory);
            OnBattleEnd?.Invoke(BattleResult.Victory);
        }
        else if (!player.IsAlive())
        {
            Debug.Log("游戏失败");
            ChangeState(BattleState.Defeat);
            OnBattleEnd?.Invoke(BattleResult.Defeat);
        }
    }

    /// <summary>
    /// 开始下一回合，执行回合序列
    /// </summary>
    public void NextTurn()
    {
        StartCoroutine(ExecuteTurnSequence());
    }

    /// <summary>
    /// 执行回合序列：玩家回合结束 -> 敌人回合 -> 玩家回合开始
    /// </summary>
    /// <returns>协程迭代器</returns>
    private IEnumerator ExecuteTurnSequence()
    {
        yield return StartCoroutine(PlayerTurnEndCoroutine());
        yield return StartCoroutine(EnemyTurnCoroutine());
        yield return StartCoroutine(PlayerTurnStartCoroutine());
    }

    /// <summary>
    /// 玩家回合结束阶段：弃掉手牌，清除护盾
    /// </summary>
    /// <returns>协程迭代器</returns>
    private IEnumerator PlayerTurnEndCoroutine()
    {
        ChangeState(BattleState.PlayerTurnEnd);
        CardManager.Instance.TurnEnd();
        player.ClearBlock();
        Debug.Log("结算回合结束效果");
        yield return new WaitForSeconds(0.5f);
    }

    /// <summary>
    /// 敌人回合阶段：逐个执行敌人意图
    /// </summary>
    /// <returns>协程迭代器</returns>
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

    /// <summary>
    /// 玩家回合开始阶段：恢复能量、抽牌、清除护盾
    /// </summary>
    /// <returns>协程迭代器</returns>
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
