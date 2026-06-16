using Assets._Project.Scripts.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("角色引用")]
    public Player player;
    public List<Enemy> enemies;          // 当前战斗中的敌人

    [Header("卡牌系统")]     // 管理牌库、手牌、弃牌堆
    public BattleState currentState;
    [Header("怪物")]
    public GameObject enemyPrefab;            // 敌人预制体
    public Transform enemySpawnPoint;         // 场景中敌人出生点
    public void ChangeState(BattleState newState) { }
    public void InitBattle()
    {
        // 1. 根据关卡配置创建敌人（下面细讲）
        GameObject enemyObj = Instantiate(enemyPrefab, enemySpawnPoint);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemies.Add(enemy);
        // 配置敌人属性
        enemy.maxHealth = 50;
        enemy.currentHealth = 50;
        // 2. 给 player 初始化血量、能量

        if (player != null)
        {
            // 30血量、3点最大能量
            player.InitPlayer(30, 3);
            player.ResetEnergy();
        }
        // 3. cardManager.InitDeck(player.deckData); // 从卡组配置抽牌
        CardManager.Instance.DisCard(5);
    }
    // 这里就是你要的“使用卡牌”的业务逻辑
    public void PlayCard(Card card, Character target)
    {
        // 只能在玩家回合中使用
        if (currentState != BattleState.PlayerAction) return;
        if (player.energy < card.cost) return;

        // 1. 消耗能量
        player.SpendEnergy(card.cost);
        // 2. 执行卡牌效果
        card.Play(player, target);
        // 3. 从手牌移到弃牌堆
        CardManager.Instance.DiscardCard(card);
        // 4. 检查胜负
        CheckBattleEnd();
        BattleUIManager.Instance.RefreshAllUI();
    }
    void CheckBattleEnd() { }
    //卡组三分区：牌库、手牌、弃牌堆
    public Button button;
    public static BattleManager Instance {  get; private set; }
    private void Awake()
    {
        //InitBattle();
        if (Instance) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void NextTurn()
    {

        PlayerTurnEnd();
        EnemyTurnStart();
        PlayerTurnStart();
        PlayerAction();
        BattleUIManager.Instance.RefreshAllUI();
    }
    public void PlayerTurnEnd()
    {
        // 结算回合结束效果
        currentState = BattleState.PlayerTurnEnd;
        Debug.Log("结算回合结束效果");
    }
    public void EnemyTurnStart()
    {
        // 敌人执行意图
        currentState = BattleState.EnemyTurnStart;
        Debug.Log("敌人执行意图");

    }
    public void PlayerTurnStart()
    {
        // 回能量、抽牌、重置格挡,结算敌方回合结束效果
        Debug.Log("回能量、抽牌、重置格挡,结算敌方回合结束效果");
        currentState = BattleState.PlayerTurnStart;

    }
    public void PlayerAction()
    {
        // 等待玩家出牌
        currentState = BattleState.PlayerAction;
        Debug.Log("玩家出牌");

    }
    //初始化战斗
    /*PlayerTurnStart,   // 回能量、抽牌、重置格挡
        PlayerAction,      // 等待玩家出牌
        PlayerTurnEnd,     // 结算回合结束效果
        EnemyTurnStart,    // 敌人执行意图
        EnemyAction,       // 播放动作
        EnemyTurnEnd,      // 结算敌方回合结束效果*/

    //构建初始卡组（8打击+5防御）
}