using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Scripts.Managers
{
    public class CardManager : MonoBehaviour
    {
        public List<Card> deck;
        public List<Card> discardPile;
        public List<Card> hand;
        public int handSize = 5;

        public void DrawCards(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (deck.Count == 0)
                    ShuffleDiscardIntoDeck();
                if (deck.Count == 0) break;

                Card drawn = deck[0];
                deck.RemoveAt(0);
                hand.Add(drawn);
            }
        }

        void ShuffleDiscardIntoDeck()
        {
            deck.AddRange(discardPile);
            discardPile.Clear();
            // 洗牌算法
            for (int i = 0; i < deck.Count; i++)
            {
                int randIdx = Random.Range(i, deck.Count);
                (deck[i], deck[randIdx]) = (deck[randIdx], deck[i]);
            }
        }
    }
}
