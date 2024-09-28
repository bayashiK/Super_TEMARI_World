using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UniRx;
using TEMARI.Model;

namespace TEMARI.View
{
    public class TemariSD : SDBase
    {
        /// <summary> 攻撃可能回数 </summary>
        public int AttackCapacity { get; private set; }

        /// <summary> 残弾数 </summary>
        private int _remainingWord;

        /// <summary> リロード時間(秒) </summary>
        public float ReloadTime { get; private set; }

        /// <summary> 残弾数通知イベント </summary>
        public IObservable<int> RemainingWordNum => _remainingWordNum;
        private Subject<int> _remainingWordNum = new();

        protected override void Awake()
        {
            base.Awake();
            destination *= -1;
            knockBackBody *= -1;
            knockBackWord *= -1;
            AttackCapacity = 3;
            _remainingWord = AttackCapacity;
            ReloadTime = 2;
            wordList = new List<string>() { "足を引っ張ったら\n殺すから", "低俗だね", "いらいらするな", "馬鹿じゃないの" };
        }

        /// <summary>
        /// 言葉を発射して攻撃
        /// </summary>
        public override async void Attack()
        {
            if (isKnockBack || isDown || _remainingWord <= 0)
            {
                return;
            }

            await KillMoveTween(0.2f);
            _ = Rect.DOLocalJump(Rect.localPosition, 40, 1, 0.4f);

            cts = new();
            try
            {
                SetActive(SpriteType.Special);
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
        /// 勝敗のセット
        /// </summary>
        /// <param name="win"></param>
        public override void SetResult(bool win)
        {
            cts.Cancel();
            _ = KillMoveTween(0);
            if (win)
            {
                SetActive(SpriteType.Special);
            }
            else
            {
                SetActive(SpriteType.Down);
                result.OnNext(false);
            }
        }

        /// <summary>
        /// 言葉攻撃プレハブの生成
        /// </summary>
        /// <returns></returns>
        protected async override UniTask InstantiateWord()
        {
            var obj = await wordPrefab.InstantiateAsync(this.transform.parent);
            var objRect = obj.transform as RectTransform;
            objRect.localPosition = Rect.localPosition + new Vector3(-knockBackBody, UnityEngine.Random.Range(-20, 20) + defaultY, 0);
            var word = obj.GetComponent<Word>();
            var text = wordList[UnityEngine.Random.Range(0, wordList.Count)];
            word.Init(_collider, text, -startLine, wordSpeed, attack);
            _remainingWord--;
            _remainingWordNum.OnNext(_remainingWord);
            if (_remainingWord <= 0)
            {
                _ = Reload();
            }
        }

        /// <summary>
        /// リロード
        /// </summary>
        private async UniTask Reload()
        {
            
            await UniTask.Delay(TimeSpan.FromSeconds(ReloadTime));
            _remainingWord = AttackCapacity;
        }

        /// <summary>
        /// 体当たり被弾
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected async override UniTask HitBody(GameObject obj)
        {
            SoundManager.Instance.PlaySE(SoundManager.SEType.BodyAttack);
            if (isDown)
            {
                var enemy = obj.GetComponent<SDBase>();
                int damage = Mathf.Clamp(enemy.GetAttack() - defence, 1, hp);
                hp = Mathf.Clamp(hp - damage, 0, hp);
                onDamaged.OnNext(hp);
                effectManager.Play(EffectType.Hit);

                if (hp <= 0)
                {
                    cts.Cancel();
                    ctsDown.Cancel();
                    SetResult(false);
                }
            }
            else
            {
                await KnockBack(knockBackBody, cts.Token);
            }
        }
    }
}