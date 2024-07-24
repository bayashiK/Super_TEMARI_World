using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System;
using UniRx;
using System.Collections.Generic;
using DG.Tweening;

namespace TEMARI.View
{
    /// <summary>
    /// トランプビュー管理クラス
    /// </summary>
    public abstract class TrumpViewBase : MonoBehaviour
    {
        /// <summary> カードプレハブの参照 </summary>
        [SerializeField] protected AssetReferenceGameObject cardPrefab;
        /// <summary> スプライトアトラスの参照 </summary>
        [SerializeField] protected AssetReferenceT<SpriteAtlas> cardSpritesRef;
        /// <summary> トランプスプライトアトラス </summary>
        protected SpriteAtlas cardSprites;
        /// <summary> カード生成時のタスクリスト </summary>
        protected List<UniTask> instantiateTaskLis = new();

        /// <summary> プレイヤーの手札 </summary>
        protected List<TrumpCard> playerCards = new();
        /// <summary> プレイヤーの手札位置 </summary>
        protected List<Vector2> playerCardsPos = new();

        /// <summary> 対戦相手の手札 </summary>
        protected List<TrumpCard> opponentCards = new() ;
        /// <summary> 対戦相手の手札位置 </summary>
        protected List<Vector2> opponentCardsPos = new();

        /// <summary> プレイヤー手札の親オブジェクト </summary>
        [SerializeField] protected Transform playerParent;
        /// <summary> 対戦相手手札の親オブジェクト </summary>
        [SerializeField] protected Transform opponentParent;

        /// <summary> 手札のいずれかドラッグ移動完了時通知 </summary>
        public IObservable<int> OnSelected => onSelected;
        protected Subject<int> onSelected = new();

        /// <summary> エラー通知イベント </summary>
        public IObservable<string> OnError => onError;
        protected Subject<string> onError = new();

        protected virtual async UniTask Start()
        {
            _ = onError.AddTo(this);
            var handle = cardSpritesRef.LoadAssetAsync<SpriteAtlas>();
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                cardSprites = handle.Result;
            }
            else
            {
                Debug.Log("failed");
                cardSprites = null;
                onError.OnNext("データの読み込みで\nエラーが発生したため\nタイトルに戻ります");
                return;
            }
        }

        /// <summary>
        /// 破棄時にマスタ解放
        /// </summary>
        protected void OnDestroy()
        {
            if (cardSprites != null)
            {
                //Debug.Log("master released");
                Addressables.Release(cardSprites);
            }
        }

        /// <summary>
        /// ゲーム開始時処理
        /// </summary>
        /// <param name="playerHand"></param>
        /// <param name="opponentHand"></param>
        /// <returns></returns>
        public virtual async UniTask StartGame(List<string> playerHand, List<string> opponentHand)
        {
            await InstantiateCards(playerHand, opponentHand);
        }

        /// <summary>
        /// お互いの手札カードプレハブ生成（一括）
        /// </summary>
        /// <param name="playerHand"></param>
        /// <param name="opponentHand"></param>
        /// <returns></returns>
        protected async UniTask InstantiateCards(List<string> playerHand, List<string> opponentHand)
        {
            var pHandles = new List<AsyncOperationHandle<GameObject>>();
            var oHandles = new List<AsyncOperationHandle<GameObject>>();
            var tasks = new List<UniTask>();
            //プレハブインスタンス化
            for (int i = 0; i < playerHand.Count; i++)
            {
                var handle = cardPrefab.InstantiateAsync();
                pHandles.Add(handle);
                tasks.Add(handle.ToUniTask());
            }
            for (int i = 0; i < opponentHand.Count; i++)
            {
                var handle = cardPrefab.InstantiateAsync();
                oHandles.Add(handle);
                tasks.Add(handle.ToUniTask());
            }
            await tasks;

            //親オブジェクト、スプライトセット、イベント購読など初期設定処理
            for (int i = 0; i < playerHand.Count; i++)
            {
                await InitCard(pHandles[i].Result, i, playerHand[i], true);
            }
            for (int i = 0; i < opponentHand.Count; i++)
            {
                await InitCard(oHandles[i].Result, i, opponentHand[i], false);
            }
        }

        /// <summary> カードの共通初期設定 </summary>
        protected async UniTask InitCard(GameObject prefab, int index, string name, bool isPlayer)
        {
            prefab.transform.SetParent(playerParent, false);
            var trumpCard = prefab.GetComponent<TrumpCard>();
            trumpCard.SetFaceSprite(cardSprites.GetSprite(name));
            trumpCard.HandIndex = index;
            if (isPlayer)
            {
                playerCards.Add(trumpCard);
                await InitPlayerCard(trumpCard);
            }
            else
            {
                opponentCards.Add(trumpCard);
                await InitOpponentCard(trumpCard);
            }
        }

        /// <summary> プレイヤーカードの初期設定 </summary>
        protected abstract UniTask InitPlayerCard(TrumpCard card);

        /// <summary> 相手カードの初期設定 </summary>
        protected abstract UniTask InitOpponentCard(TrumpCard card);

        /// <summary>
        /// カードプレハブ生成（単体）
        /// </summary>
        /// <param name="isPlayer"></param>
        public async UniTask InstantiateCard(string name, bool isPlayer)
        {
            var handle = cardPrefab.InstantiateAsync();
            if (isPlayer) 
            {
                await handle.Task;
                //await InitCard(handle.Result, i, name, true);
            }
            else
            {
                await handle.Task;
                //await InitCard(handle.Result, i, name, false);
            }
        }

        /// <summary>
        /// カードプレハブ削除
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isPlayer"></param>
        protected void DestroyCard(TrumpCard card)
        {
            if (playerCards.Contains(card))
            {
                playerCards.Remove(card);
            }
            else
            {
                opponentCards.Remove(card);
            }
            Addressables.ReleaseInstance(card.gameObject);
        }

        /// <summary>
        /// 全カードプレハブ削除
        /// </summary>
        public void DestroyAllCard()
        {
            foreach (var card in playerCards)
            {
                Addressables.ReleaseInstance(card.gameObject);
            }
            foreach (var card in opponentCards)
            {
                Addressables.ReleaseInstance(card.gameObject);
            }
            playerCards.Clear();
            opponentCards.Clear();
        }

        /// <summary>
        /// カードのイベント受付可否一括設定
        /// </summary>
        /// <param name="isPlayer">プレイヤー側の手札を対象とするか</param>
        /// <param name="enabled">イベント受付可否</param>
        public void SetTriggerEnabled(bool isPlayer, bool enabled)
        {
            List<TrumpCard> cards = isPlayer ? playerCards : opponentCards;
            foreach (var card in cards)
            {
                card.IsTriggerEnabled = enabled;
            }
        }
        
        /*
        /// <summary>
        /// カードの位置入れ替え
        /// </summary>
        /// <param name="card"></param>
        /// <param name="draggedPos"></param>
        protected async UniTask SwapCard(TrumpCard card)
        {
            Vector2 currentPos = card.AnchoredPos;
            int currentInd = playerCards.IndexOf(card);
            int ind = 0;
            float minDistance = Mathf.Infinity;
            for (int i = 0; i < playerCardsPos.Count; i++)
            {
                var distance = Vector2.Distance(currentPos, playerCardsPos[i]);
                if (distance < minDistance)
                {
                    ind = i;
                    minDistance = distance;
                }
            }

            if (currentInd == ind) { }
            else
            {
                if (currentPos.x > playerCardsPos[ind].x)
                {
                    if (ind == playerCardsPos.Count - 1)
                    {
                        playerCards.Remove(card);
                        playerCards.Add(card);
                        goto MOVE;
                    }
                    else ind++;
                }
                else
                {
                    if (ind == 0)
                    {
                        playerCards.Remove(card);
                        playerCards.Insert(0, card);
                        goto MOVE;
                    }
                    else ind--;
                }
                playerCards.Remove(card);
                playerCards.Insert(ind, card);
            }
        MOVE:;
            card.transform.SetSiblingIndex(playerCards.IndexOf(card));
            await MoveCardsNewPos(true);
        
        */
    }
}