using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace TEMARI.View
{
    public class TemariSD : SDBase
    {
        protected override void Start()
        {
            destination *= -1;
            knockBackBody *= -1;
            knockBackWord *= -1;
            base.Start();
            wordList = new List<string>() { "足を引っ張ったら\n殺すから", "低俗だね", "いらいらするな", "馬鹿じゃないの" };
        }

        /// <summary>
        /// 言葉を発射して攻撃
        /// </summary>
        public override async void Attack()
        {
            if (isKnockBack || isDown)
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
    }
}