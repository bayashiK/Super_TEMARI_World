using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

namespace TEMARI.View
{
    /// <summary>
    /// SD戦闘モーションクラス
    /// </summary>
    public class SDBase : MonoBehaviour
    {
        /// <summary> 走るスプライト </summary>
        [SerializeField] protected Image runSprite;

        /// <summary> ダメージを受けるスプライト </summary>
        [SerializeField] protected Image damageSprite;

        /// <summary> ダメージを受けた際のパーティクル </summary>
        [SerializeField] protected ParticleSystem damageParticle;

        /// <summary> 言葉攻撃プレハブの参照 </summary>
        [SerializeField] protected AssetReferenceGameObject wordPrefab;

        /// <summary> 移動速度 </summary>
        [SerializeField] protected float moveSpeed = 50;

        /// <summary> 言葉攻撃の速度 </summary>
        [SerializeField] protected float wordSpeed = 70;

        /// <summary> コライダー </summary>
        private BoxCollider2D _collider;

        /// <summary> ダメージ通知イベント </summary>
        public IObservable<int> OnDamaged => onDamaged;
        protected Subject<int> onDamaged = new();

        /// <summary> デフォルトのy座標 </summary>
        protected float defaultY;

        /// <summary> 移動先の座標 </summary>
        protected float destination = 1000;

        /// <summary> 体当たり時のノックバック </summary>
        protected float knockBackBody = -200;

        /// <summary> 言葉攻撃を受けた際のノックバック </summary>
        protected float knockBackWord = -100;

        /// <summary> ノックバックフラグ </summary>
        protected bool isKnockBacked = false;

        /// <summary> 語彙 </summary>
        protected List<string> wordList = new();

        protected RectTransform _rect;

        private CancellationTokenSource cts = new CancellationTokenSource();

        protected virtual void Start()
        {
            damageSprite.gameObject.SetActive(false);
            _collider = this.GetComponent<BoxCollider2D>();
            _rect = this.transform as RectTransform;
            defaultY = _rect.localPosition.y;
        }
        
        /// <summary>
        /// 移動開始
        /// </summary>
        public void StartMove()
        {
            runSprite.gameObject.SetActive(true);
            damageSprite.gameObject.SetActive(false);
            var dist = Mathf.Abs(destination - _rect.localPosition.x);
            _rect.DOLocalJump(new Vector2(destination, defaultY), 30, (int)dist / 40, dist / moveSpeed);
        }

        /// <summary>
        /// 言葉を発射して攻撃
        /// </summary>
        public virtual async void Attack()
        {
            if (isKnockBacked)
            {
                return;
            }

            await KillMoveTween(0.2f);
            _ = _rect.DOLocalJump(_rect.localPosition, 40, 1, 0.4f);
            /*
            float defY = _rect.localPosition.y;
            _ = DOTween.Sequence().Append(_rect.DOLocalMoveY(defY + 40, 0.2f))
                .Append(_rect.DOLocalMoveY(defY, 0.2f).SetEase(Ease.InQuad));
            */

            cts = new();
            
            try
            {
                await UniTask.WhenAll(InstantiateWord(), UniTask.Delay(TimeSpan.FromMilliseconds(500), cancellationToken: cts.Token));
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString());
                return;
            }
            StartMove();
        }

        /// <summary>
        /// 言葉攻撃プレハブの生成
        /// </summary>
        /// <returns></returns>
        protected async UniTask InstantiateWord()
        {
            var obj = await wordPrefab.InstantiateAsync(this.transform.parent);
            obj.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
            var objRect = obj.transform as RectTransform;
            objRect.localPosition = _rect.localPosition + new Vector3(-knockBackBody, UnityEngine.Random.Range(-20, 20) + defaultY, 0);
            var word = obj.GetComponent<Word>();
            var text = wordList[UnityEngine.Random.Range(0, wordList.Count)];
            word.Init(_collider, text, destination, wordSpeed);
        }

        /// <summary>
        /// ノックバック時のトゥイーン
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual async UniTask KnockBack(GameObject obj, CancellationToken token)
        {
            isKnockBacked = true;
            var currentX = _rect.localPosition.x;
            damageParticle.Play();
            try
            {
                if (obj.CompareTag("Word"))
                {
                    obj.GetComponent<Word>().Burst();
                    _ = this.transform.DOLocalMoveX(currentX + knockBackWord, 0.3f);
                    await UniTask.Delay(TimeSpan.FromMilliseconds(300), cancellationToken: token);
                }
                else
                {
                    _ = _rect.DOLocalMoveX(currentX + knockBackWord, 0.6f);
                    await UniTask.Delay(TimeSpan.FromMilliseconds(600), cancellationToken: token);
                }
            }
            catch(OperationCanceledException e)
            {
                Debug.Log(e.ToString());
                return;
            }
            isKnockBacked = false;
            StartMove();
        }

        /// <summary>
        /// 移動トゥイーンの停止
        /// </summary>
        protected async UniTask KillMoveTween(float time)
        {
            _rect.DOKill();
            await _rect.DOLocalMoveY(defaultY, time);
        }

        /// <summary>
        /// 接触時
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            _ =  KillMoveTween(0);
            if (isKnockBacked)
            {
                cts.Cancel();
            }
            else
            {
                runSprite.gameObject.SetActive(false);
                damageSprite.gameObject.SetActive(true);
            }
            
            cts = new();
            _ = KnockBack(collision.gameObject, cts.Token);
        }
    }
}