using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UniRx;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

namespace TEMARI.Model
{
    /// <summary>
    /// ワンポーカーモデル
    /// </summary>
    public class OnePokerModel : SceneBase
    {
        /// <summary> ベットの種類 </summary>
        public enum BetType 
        {
            Call = 0,
            Raise,
            Drop
        }

        /// <summary> カードの組み合わせ </summary>
        public enum CardCombination
        {
            WUP,
            UPDOWN,
            WDOWN
        }

        /// <summary> 勝負結果 </summary>
        public enum Result
        { 
            WIN,
            LOSE,
            DRAW
        }

        /// <summary> 手札クラス </summary>
        public class OnePokerHand
        {
            private List<Trump> _cards;

            public CardCombination Combination { get; private set; }

            public OnePokerHand(Trump card1, Trump card2)
            {
                _cards.Add(card1);
                _cards.Add(card2);
                SetComb();
            }

            public List<string> GetHandName()
            {
                return new List<string>() { _cards[0].Name, _cards[1].Name };
            }

            public (int num1, int num2) GetHandNumber()
            {
                return (_cards[0].Number, _cards[1].Number);
            }

            public Trump SelectCard(int i)
            {
                if (_cards.Count != 2) return null;
                var card = _cards[i];
                _cards.Remove(card);
                return card;
            }

            public void AddCard(Trump card)
            {
                if (_cards.Count != 1) return;
                _cards.Add(card);
                SetComb();
            }

            /// <summary> カードの組み合わせ情報セット </summary>
            private void SetComb()
            {
                int n = JudgeUpDown(_cards[0]) + JudgeUpDown(_cards[1]);
                Combination = n switch
                {
                    0 => CardCombination.WDOWN,
                    1 => CardCombination.UPDOWN,
                    2 => CardCombination.WUP,
                    _ => CardCombination.UPDOWN  //本来あり得ない
                };
            }

            /// <summary> トランプの数字がUPならtrueを返す </summary>
            private int JudgeUpDown(Trump card)
            {
                return ((card.Number == 1) || (card.Number >= 8)) ? 1 : 0;
            }
        }

        public bool IsBetting { get; set; } = false;

        /// <summary> 最低ベット金額 </summary>
        public readonly int MinBet = 100;
        /// <summary> 現在のベット金額 </summary>
        public int CurrentBet { get; set; }

        private TrumpManager _tm;

        /// <summary> プレイヤーの手札 </summary>
        private OnePokerHand _playerHand;

        /// <summary> 対戦相手の手札 </summary>
        private OnePokerHand _opponentHand;

        /// <summary> ゲーム回数 </summary>
        public int GameNum { get; set; }

        /// <summary> 現在が何回戦目か </summary>
        public int GameCount { get; private set; } = 0;

        /// <summary> 手札内容通知 </summary>
        public IObservable<(OnePokerHand pHand, OnePokerHand oHand)> OnPicked => _onPicked;
        private Subject<(OnePokerHand, OnePokerHand)> _onPicked = new();

        /// <summary> カードめくり通知 </summary>
        public IObservable<bool> OnTurnCard => _onTurnCard;
        private Subject<bool> _onTurnCard = new();

        /// <summary> ゲーム終了通知 </summary>
        public IObservable<bool> OnGameEnd => _onGameEnd;
        private Subject<bool> _onGameEnd = new();

        protected override void Start()
        {
            base.Start();
            _onPicked.AddTo(this);
            _onGameEnd.AddTo(this);
            _tm = new TrumpManager();
        }

        /// <summary>
        /// ゲームスタート
        /// </summary>
        public void StartGame()
        {
            GameNum = 3;
            GameCount = 0;
            _tm.GenerateDeck(false);
            _tm.ShuffledDeck();
            _playerHand = new OnePokerHand(_tm.GetCard(), _tm.GetCard());
            _opponentHand = new OnePokerHand(_tm.GetCard(), _tm.GetCard());
            StartTurn();
        }

        /// <summary>
        /// ターンスタート
        /// </summary>
        public void StartTurn()
        {
            GameCount++;
            if (GameCount > 1) PickCard();
            CurrentBet = MinBet;
            _onPicked.OnNext((_playerHand, _opponentHand));
        }

        private void EndTurn()
        {
            if(GameCount >= GameNum)
            {
                _onGameEnd.OnNext(true);
            }
            else
            {
                if(basicData.Money >= MinBet)
                {

                }
                else
                {
                    _onGameEnd.OnNext(true);
                }
            }
        }

        /// <summary>
        /// お互いにカードを引く
        /// </summary>
        /// <param name="isPlayer"></param>
        private void PickCard()
        {
            _playerHand.AddCard(_tm.GetCard());
            _opponentHand.AddCard(_tm.GetCard());
            _onPicked.OnNext((_playerHand, _opponentHand));
        }

        /// <summary>
        /// プレイヤーのカード選択
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void SelectPlayerCard(int index)
        {
            IsBetting = true;
            var pCard = _playerHand.SelectCard(index);
            var oCard = SelectOpponentCard();
            var result =  JudgeResult(pCard, oCard);
        }

        /// <summary>
        /// 相手のカード選択
        /// </summary>
        /// <returns></returns>
        private Trump SelectOpponentCard()
        {
            var i = UnityEngine.Random.Range(0, 1);
            return _opponentHand.SelectCard(i);
        }

        /// <summary>
        /// プレイヤーのベット種類選択
        /// </summary>
        /// <param name="bet"></param>
        /// <param name="amount"></param>
        public void SelectPlayerBet(BetType bet, int amount)
        {
            if(bet == BetType.Raise)
            {
                CurrentBet = amount;
            }
            else if(bet == BetType.Drop)
            {
                basicData.Money = -amount;
                IsBetting = false;
                EndTurn();
            }
            var oBet = SelectOpponentBet(bet);
        }

        /// <summary>
        /// 相手のベット種類選択
        /// </summary>
        /// <param name="pBet"></param>
        /// <returns></returns>
        private BetType SelectOpponentBet(BetType pBet)
        {
            var i = UnityEngine.Random.Range(0, 2);
            var oBet =  (BetType)Enum.ToObject(typeof(BetType), i);
            allText = oBet switch
            {
                BetType.Call => new List<string>() { "Call" },
                BetType.Raise => new List<string>() { "Raise" },
                _ => new List<string>() { "Drop" }
            };
            //isTextEnabled.Value = View.TextType.Fukidashi;
            return oBet;
        }

        /// <summary>
        /// 勝負の結果判定
        /// </summary>
        /// <param name="pCard"></param>
        /// <param name="oCard"></param>
        private Result JudgeResult(Trump pCard, Trump oCard)
        {
            if(pCard.IsSameNumber(oCard))  //数字が同じなら引き分け
            {
                return Result.DRAW;
            }
            else if(pCard.Number == 1 && oCard.Number != 2)  //自分がAなら２以外に勝てる
            {
                return Result.WIN;
            }
            else if(pCard.Number != 2 && oCard.Number == 1)  //相手がAなら２でないと負ける
            {
                return Result.LOSE;
            }
            else  //それ以外は数字が大きいほうが勝ち
            {
                if (pCard.Number > oCard.Number)
                {
                    return Result.WIN;
                }
                else
                {
                    return Result.LOSE;
                }
            }
        }
    }
}