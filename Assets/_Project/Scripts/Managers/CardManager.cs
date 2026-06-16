using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Scripts.Managers
{
    public class CardManager : MonoBehaviour
    {
        public List<Card> drawcardPile = new List<Card> { };
        public List<Card> discardPile = new List<Card> { };
        public List<Card> hand = new List<Card> { };
        public int handSize = 5;
        public Card cardAtk1;
        public Card cardAtk2;
        public Card cardAtk10;
        public static CardManager Instance;
        void Awake()
        {
            if (Instance) Destroy(gameObject);
            else Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public void AddCard(List<Card> cardList)
        {
            drawcardPile.AddRange(cardList);
        }
        public void ReCard(Card card)
        {
            drawcardPile.Remove(card);
        }
        public void InitGame()
        {
            drawcardPile.Clear();
            discardPile.Clear();
            hand.Clear();
            // 1攻击卡 10张
            for (int i = 0; i < 10; i++)
            {
                drawcardPile.Add(cardAtk1);
            }
            // 2攻击卡 10张
            for (int i = 0; i < 10; i++)
            {
                drawcardPile.Add(cardAtk2);
            }
            // 10攻击卡 10张
            for (int i = 0; i < 10; i++)
            {
                drawcardPile.Add(cardAtk10);
            }
            Debug.Log("当前卡组数量：" + drawcardPile.Count);
        }
        public void InitBattle()
        {
            drawcardPile.AddRange(hand);
            drawcardPile.AddRange(discardPile);
            hand.Clear();
            discardPile.Clear();
            DisCard(5);
        }
        public void DisCard(int n)
        {
            if (drawcardPile.Count < n)
            {
                ShuffleDiscardIntoDeck();
                if(drawcardPile.Count < n)
                {
                    n= drawcardPile.Count;
                    //TestWindow win = UIFrame.Instance.OpenDynamicWindow(testWindow) as TestWindow;
                    //if (win != null)
                    //win.Initialize("动态窗口 " + Random.Range(1, 100));
                    Debug.Log("手牌数量不足");
                }
            }
            for(int i = 0;i<n;i++)
            {
                if(drawcardPile.Count==0)
                {
                    ShuffleDiscardIntoDeck();
                }
                hand.Add(drawcardPile[0]);
                drawcardPile.RemoveAt(0);
            }
            
        }
        public void DiscardCard(Card card)
        {
            if (!hand.Contains(card)) return; // 安全检查
            hand.Remove(card);
            discardPile.Add(card);
        }

        void ShuffleDiscardIntoDeck()
        {
            drawcardPile.AddRange(discardPile);
            discardPile.Clear();
            // 洗牌算法
            for (int i = 0; i < drawcardPile.Count; i++)
            {
                int randIdx = Random.Range(i, drawcardPile.Count);
                (drawcardPile[i], drawcardPile[randIdx]) = (drawcardPile[randIdx], drawcardPile[i]);
            }
        }
    }
}
