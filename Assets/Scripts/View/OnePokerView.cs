using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using System;

namespace TEMARI.View
{
    public class OnePokerView : TrumpViewBase
    {
        public int MaxBet { get; set; } = 0;

        private int _playerSelect;

        protected override async UniTask Start()
        {
            await base.Start();
        }

        public override async UniTask StartGame(List<string> playerHand, List<string> opponentHand)
        {
            await base.StartGame(playerHand, opponentHand);
            await playerCards[0].TurnCard();
            await playerCards[1].TurnCard();
            SetTriggerEnabled(true, true);

        }

        protected override async UniTask InitPlayerCard(TrumpCard card)
        {
            float posx = card.HandIndex == 0 ? -30f : 30f;
            await card.Move(new Vector2(posx, 0), 0.2f);
            await card.TurnCard();
            card.OnDragged
                .Subscribe(async vec =>
                {
                    if(vec.y > 0.6f)
                    {
                        _playerSelect = card.HandIndex;
                        await card.TurnCard();
                        await card.Move(new Vector2(0, 200), 0.3f);
                        onSelected.OnNext(_playerSelect);
                    }
                    else
                    {
                        card.UndoDrag();
                    }
                })
                .AddTo(card);
        }

        protected override async UniTask InitOpponentCard(TrumpCard card)
        {
            float posx = card.HandIndex == 0 ? -30f : 30f;
            await card.Move(new Vector2(posx, 0), 0.2f);
        }

        /// <summary>
        /// デフォルトの位置に手札セット
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="yPlayer"></param>
        /// <param name="yOpponent"></param>
        /// <returns></returns>
        private async UniTask SetDefaultPos()
        {
            UpdateCardsPos(playerCards, playerCardsPos);
            UpdateCardsPos(opponentCards, opponentCardsPos);

            var playerSequence = DOTween.Sequence();
            for (int i = 0; i < playerCards.Count; i++)
            {
                _ = playerSequence.Append(playerCards[i].Move(playerCardsPos[i], 0.1f));
            }
            var opponentSequence = DOTween.Sequence();
            for (int i = 0; i < opponentCards.Count; i++)
            {
                _ = opponentSequence.Append(opponentCards[i].Move(opponentCardsPos[i], 0.1f));
            }
            var sequence = playerSequence.Join(opponentSequence);
            await sequence;
        }

        /// <summary>
        /// 更新後の位置にカード移動
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="yPlayer"></param>
        /// <param name="yOpponent"></param>
        /// <returns></returns>
        private async UniTask MoveCardsNewPos(bool isPlayer)
        {
            List<TrumpCard> cards = isPlayer ? playerCards : opponentCards;
            List<Vector2> cardsPos = isPlayer ? playerCardsPos : opponentCardsPos;
            UpdateCardsPos(cards, cardsPos);
            var sequence = DOTween.Sequence();
            for (int i = 0; i < cards.Count; i++)
            {
                _ = sequence.Join(cards[i].Move(cardsPos[i], 0.1f));
            }
            await sequence;
        }

        /// <summary>
        /// カードの位置更新
        /// </summary>
        /// <param name="xMax"></param>
        /// <param name="yPlayer"></param>
        /// <param name="yOpponent"></param>
        private void UpdateCardsPos(List<TrumpCard> cards, List<Vector2> cardsPos)
        {
            cardsPos.Clear();
            var cardDistance = 800f / (cards.Count - 1);
            for (int i = 0; i < cards.Count; i++)
            {
                cardsPos.Add(new Vector2(cardDistance * i - 400f, 0));
            }
        }

        public void ShowCard()
        {

        }
    }
}