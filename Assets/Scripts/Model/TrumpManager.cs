using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TEMARI.Model
{
    /// <summary> トランプクラス </summary>
    public class Trump
    {
        /// <summary> スートの頭文字 + 数字 </summary>
        public string Name { get; }
        public Suit Suit { get; }
        /// <summary> Joker:0, 1～13 </summary>
        public int Number { get; }

        public Trump(Suit suit, int number)
        {
            Suit = suit;
            Number = suit == Suit.Joker ? 0 : Mathf.Clamp(number, 1, 13);
            Name = suit == Suit.Joker ? "Joker" : Suit.ToString().Substring(0, 1) + Number.ToString();
        }

        /// <summary>
        /// 数字が同じか判定
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsSameNumber(Trump card)
        {
            return Number == card.Number;
        }

        public bool Equals(Trump trump)
        {
            return (this.Suit == trump.Suit) && IsSameNumber(trump);
        }
    }

    /// <summary> スート </summary>
    public enum Suit
    {
        Joker = 0,
        Spade,
        Heart,
        Diamond,
        Club
    }

    /// <summary>
    /// トランプ管理モデルクラス
    /// </summary>
    public class TrumpManager
    {
        /// <summary> デッキ </summary>
        public IReadOnlyCollection<Trump> Deck => _deck;
        private List<Trump> _deck;

        /// <summary> コンストラクタ </summary>
        public TrumpManager()
        {

        }

        /// <summary>
        /// デッキ生成
        /// </summary>
        public void GenerateDeck(bool inJoker)
        {
            _deck = new List<Trump>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                if (suit == Suit.Joker && inJoker)
                {
                    _deck.Add(new Trump(suit, 0));
                }
                else
                {
                    for (int i = 1; i <= 13; i++)
                    {
                        _deck.Add(new Trump(suit, i));
                    }
                }
            }
        }

        /// <summary>
        /// デッキをシャッフルする
        /// </summary>
        /// <returns></returns>
        public void ShuffledDeck()
        {
            _deck = _deck.OrderBy(_ => Guid.NewGuid()).ToList();
        }

        /// <summary>
        /// デッキから条件に合うトランプを返す
        /// </summary>
        /// <param name="suit"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Trump SearchCard(Suit suit, int num)
        {
            return _deck
                .Where(x => x.Suit == suit && x.Number == num)
                .FirstOrDefault();
        }

        /// <summary>
        /// デッキの一番上のカードを渡し、デッキから取り除く
        /// </summary>
        /// <returns></returns>
        public Trump GetCard()
        {
            var card = _deck[0];
            _deck.Remove(card);
            return card;
        }

        /// <summary>
        /// ジョーカー以外でランダムな異なるスートのペアを返す
        /// </summary>
        /// <returns></returns>
        public (Suit, Suit) GetSuitPair()
        {
            var tmp = new List<Suit>() { Suit.Spade, Suit.Heart, Suit.Diamond, Suit.Club };
            tmp = tmp.OrderBy(_ => Guid.NewGuid()).ToList();
            while (tmp.Count > 2)
            {
                var i = UnityEngine.Random.Range(0, tmp.Count);
                tmp.RemoveAt(i);
            }
            return (tmp[0], tmp[1]);
        }

        /// <summary>
        /// デッキを2つに分割
        /// </summary>
        /// <returns></returns>
        public (List<Trump> playerHand, List<Trump>opponentHand) DistributeDeck()
        {
            var half = _deck.Count / 2;
            return (_deck.GetRange(0, half), _deck.GetRange(half, _deck.Count - half));
        }

        /// <summary>
        /// 手札から重複カードを削除
        /// </summary>
        /// <param name="hand"></param>
        public void RemoveDuplicate(List<Trump> hand)
        {
            int ind = 0;
            while (ind < hand.Count - 1) {
                var card = hand[ind];
                ind++;
                for (int i = ind; i < hand.Count; i++)
                {
                    if (card.IsSameNumber(hand[i]))
                    {
                        hand.RemoveAt(i);
                        hand.RemoveAt(ind - 1);
                        ind = 0;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// トランプリストの内容出力
        /// </summary>
        /// <param name="cards"></param>
        public static void PrintTrumpList(List<Trump> cards)
        {
            if(cards == null)
            {
                Debug.Log("list is null");
                return;
            }
            else if(cards.Count == 0)
            {
                Debug.Log("list is Empty");
                return;
            }
            string str = "";
            foreach(var card in cards)
            {
                str += card.Name + ", ";
            }
            Debug.Log(str);
        }
    }
}