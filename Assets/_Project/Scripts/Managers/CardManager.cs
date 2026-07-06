﻿﻿﻿﻿﻿﻿﻿﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Scripts.Managers
{
    /// <summary>
    /// 卡牌管理器，负责管理卡组、手牌、弃牌堆的状态和操作
    /// 采用单例模式，确保全局唯一
    /// </summary>
    public class CardManager : MonoBehaviour
    {
        /// <summary>
        /// 抽牌堆，存储当前可抽取的卡牌
        /// </summary>
        public List<Card> drawcardPile = new List<Card> { };

        /// <summary>
        /// 弃牌堆，存储已使用或弃置的卡牌
        /// </summary>
        public List<Card> discardPile = new List<Card> { };

        /// <summary>
        /// 手牌堆，存储当前玩家手中的卡牌
        /// </summary>
        public List<Card> hand = new List<Card> { };

        /// <summary>
        /// 手牌上限数量，默认每回合抽5张
        /// </summary>
        public int handSize = 5;

        /// <summary>
        /// 1点攻击力卡牌模板，用于初始化卡组
        /// </summary>
        public Card cardAtk1;

        /// <summary>
        /// 2点攻击力卡牌模板，用于初始化卡组
        /// </summary>
        public Card cardAtk2;

        /// <summary>
        /// 10点攻击力卡牌模板，用于初始化卡组
        /// </summary>
        public Card cardAtk10;

        /// <summary>
        /// 卡牌管理器单例实例
        /// </summary>
        public static CardManager Instance;

        /// <summary>
        /// 初始化单例，确保场景切换时不被销毁
        /// </summary>
        void Awake()
        {
            if (Instance) Destroy(gameObject);
            else Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 向抽牌堆添加卡牌列表
        /// </summary>
        /// <param name="cardList">要添加的卡牌列表</param>
        public void AddCard(List<Card> cardList)
        {
            drawcardPile.AddRange(cardList);
        }

        /// <summary>
        /// 从抽牌堆移除指定卡牌
        /// </summary>
        /// <param name="card">要移除的卡牌</param>
        public void ReCard(Card card)
        {
            drawcardPile.Remove(card);
        }

        /// <summary>
        /// 初始化游戏卡组，创建初始卡牌并放入弃牌堆后洗牌
        /// </summary>
        public void InitGame()
        {
            drawcardPile.Clear();
            discardPile.Clear();
            hand.Clear();
            // 1攻击卡 10张
            for (int i = 0; i < 10; i++)
            {
                discardPile.Add(cardAtk1);
            }
            // 2攻击卡 10张
            for (int i = 0; i < 10; i++)
            {
                discardPile.Add(cardAtk2);
            }
            // 10攻击卡 10张
            for (int i = 0; i < 10; i++)
            {
                discardPile.Add(cardAtk10);
            }
            ShuffleDiscardIntoDeck();
            Debug.Log("当前卡组数量：" + drawcardPile.Count);
        }

        /// <summary>
        /// 初始化战斗卡组，清空所有牌堆并重新生成卡牌
        /// </summary>
        public void InitBattle()
        {
            Debug.Log($"[CardManager] InitBattle - before: hand={hand.Count}, draw={drawcardPile.Count}, discard={discardPile.Count}");
            drawcardPile.Clear();
            hand.Clear();
            discardPile.Clear();
            for (int i = 0; i < 10; i++) drawcardPile.Add(cardAtk1);
            for (int i = 0; i < 10; i++) drawcardPile.Add(cardAtk2);
            for (int i = 0; i < 10; i++) drawcardPile.Add(cardAtk10);
            ShuffleDiscardIntoDeck();
            Debug.Log($"[CardManager] InitBattle - after: hand={hand.Count}, draw={drawcardPile.Count}, discard={discardPile.Count}");
        }

        /// <summary>
        /// 回合结束，将手牌全部放入弃牌堆并清空手牌
        /// </summary>
        public void TurnEnd()
        {
            discardPile.AddRange(hand);
            hand.Clear();
        }

        /// <summary>
        /// 回合开始，抽指定数量的卡牌到手牌
        /// </summary>
        public void TurnStart()
        {
            Debug.Log($"[CardManager] TurnStart - handSize={handSize}, before: hand={hand.Count}, draw={drawcardPile.Count}");
            DisCard(handSize);
            Debug.Log($"[CardManager] TurnStart - after: hand={hand.Count}, draw={drawcardPile.Count}");
        }

        /// <summary>
        /// 抽指定数量的卡牌到手牌，若抽牌堆不足则自动洗入弃牌堆
        /// </summary>
        /// <param name="n">要抽取的卡牌数量</param>
        public void DisCard(int n)
        {
            Debug.Log($"[CardManager] DisCard({n}) - before: hand={hand.Count}, draw={drawcardPile.Count}, discard={discardPile.Count}");
            if (drawcardPile.Count < n)
            {
                Debug.Log($"[CardManager] DisCard - draw pile too small ({drawcardPile.Count}), shuffling discard");
                ShuffleDiscardIntoDeck();
                if(drawcardPile.Count < n)
                {
                    n= drawcardPile.Count;
                    Debug.Log("[CardManager] DisCard - hand count insufficient");
                }
            }
            for(int i = 0;i<n;i++)
            {
                if(drawcardPile.Count==0)
                {
                    Debug.Log("[CardManager] DisCard - draw pile empty during loop, shuffling");
                    ShuffleDiscardIntoDeck();
                }
                hand.Add(drawcardPile[0]);
                drawcardPile.RemoveAt(0);
            }
            Debug.Log($"[CardManager] DisCard({n}) - after: hand={hand.Count}, draw={drawcardPile.Count}, discard={discardPile.Count}");
        }

        /// <summary>
        /// 将指定卡牌从手牌弃置到弃牌堆
        /// </summary>
        /// <param name="card">要弃置的卡牌</param>
        public void DiscardCard(Card card)
        {
            if (!hand.Contains(card)) return;
            hand.Remove(card);
            discardPile.Add(card);
        }

        /// <summary>
        /// 将弃牌堆洗入抽牌堆并打乱顺序
        /// </summary>
        void ShuffleDiscardIntoDeck()
        {
            drawcardPile.AddRange(discardPile);
            discardPile.Clear();
            for (int i = 0; i < drawcardPile.Count; i++)
            {
                int randIdx = Random.Range(i, drawcardPile.Count);
                (drawcardPile[i], drawcardPile[randIdx]) = (drawcardPile[randIdx], drawcardPile[i]);
            }
        }
    }
}
