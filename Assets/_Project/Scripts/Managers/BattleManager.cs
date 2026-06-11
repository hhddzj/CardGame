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
    [Header("UI管理器拖拽赋值")]
    public BattleUIManager uiManager;

    //卡组三分区：牌库、手牌、弃牌堆
    public List<Card> deckList = new List<Card>();
    public List<Card> handList = new List<Card>();
    public List<Card> discardList = new List<Card>();
    public BattleState battleState = BattleState.PlayerAction;
    public Player player;
    public Enemy monster;
    private System.Random rand = new System.Random();
    public Button button;
    public static BattleManager Instance {  get; private set; }
    private void Awake()
    {
        //InitBattle();
        if (Instance) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
        button.onClick.AddListener(NextTurn);
    }
    public void NextTurn()
    {

        PlayerTurnEnd();
        EnemyTurnStart();
        PlayerTurnStart();
        PlayerAction();
    }
    public void PlayerTurnEnd()
    {
        // 结算回合结束效果
        battleState = BattleState.PlayerTurnEnd;
        Debug.Log("结算回合结束效果");
    }
    public void EnemyTurnStart()
    {
        // 敌人执行意图
        battleState = BattleState.EnemyTurnStart;
        Debug.Log("敌人执行意图");

    }
    public void PlayerTurnStart()
    {
        // 回能量、抽牌、重置格挡,结算敌方回合结束效果
        Debug.Log("回能量、抽牌、重置格挡,结算敌方回合结束效果");
        battleState = BattleState.PlayerTurnStart;

    }
    public void PlayerAction()
    {
        // 等待玩家出牌
        battleState = BattleState.PlayerAction;
        Debug.Log("玩家出牌");

    }
    //初始化战斗
    /*PlayerTurnStart,   // 回能量、抽牌、重置格挡
        PlayerAction,      // 等待玩家出牌
        PlayerTurnEnd,     // 结算回合结束效果
        EnemyTurnStart,    // 敌人执行意图
        EnemyAction,       // 播放动作
        EnemyTurnEnd,      // 结算敌方回合结束效果*/
    public void InitBattle()
    {
        //初始化角色
        player = new Player(35, 3);
        //monster = new Enemy("小怪", 55, 7);
        BuildDefaultDeck();
        DrawCard(5);
        uiManager.RefreshAllUI(this);
    }

    //构建初始卡组（8打击+5防御）
    void BuildDefaultDeck()
    {
        deckList.Clear(); discardList.Clear(); handList.Clear();
        //for (int i = 0; i < 8; i++) deckList.Add(new Card("打击", 1, 6));
        //for (int i = 0; i < 5; i++) deckList.Add(new Card("防御", 1, 0));
    }

    //抽N张牌，牌库空则洗弃牌堆
    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (deckList.Count <= 0) ShuffleDiscardToDeck();
            if (deckList.Count <= 0) break;

            int ranIdx = rand.Next(deckList.Count);
            var getCard = deckList[ranIdx];
            deckList.RemoveAt(ranIdx);
            handList.Add(getCard);
        }
    }

    //洗牌：弃牌堆进牌库，清空弃牌
    void ShuffleDiscardToDeck()
    {
        deckList.AddRange(discardList);
        discardList.Clear();
    }

    //出牌：返回true=出牌成功
    public bool PlayCard(int handIndex)
    {
        if (handIndex < 0 || handIndex >= handList.Count) return false;
        Card curCard = handList[handIndex];
        //费用不足无法出牌
        //if (player.Energy < curCard.Cost) return false;

        //扣费、怪物受伤、手牌进弃牌
        //player.Energy -= curCard.Cost;
        //monster.TakeDamage(curCard.Damage);
        handList.RemoveAt(handIndex);
        discardList.Add(curCard);
        uiManager.RefreshAllUI(this);
        CheckBattleResult();
        return true;
    }

    //结束回合按钮绑定方法
    public void EndPlayerTurn()
    {
        //全部手牌弃牌
        discardList.AddRange(handList);
        handList.Clear();

        //怪物回合：怪物攻击玩家
        //player.TakeDamage(monster.Atk);

        //新回合玩家刷新费用+抽5牌
        player.ResetEnergy();
        DrawCard(5);

        uiManager.RefreshAllUI(this);
        CheckBattleResult();
    }

    //检查胜负
    void CheckBattleResult()
    {
        //if (monster.HP <= 0) Debug.Log("战斗胜利！");
        //if (player.HP <= 0) Debug.Log("战斗失败！");
    }
}