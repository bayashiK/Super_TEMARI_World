using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Threading;

namespace TEMARI.View
{
    /// <summary>
    /// フキダシクラス
    /// </summary>
    public class Fukidashi : TextBoxBase
    {
        /// <summary> フキダシ </summary>
        private Image _fukidashi;

        /// <summary> フキダシテキスト </summary>
        private TextMeshProUGUI _fukidashiText;

        private CancellationTokenSource cts = new CancellationTokenSource();

        private Sequence sequence;

        protected override void Awake()
        {
            _fukidashi = this.GetComponent<Image>();
            _fukidashiText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
            base.Awake();
        }

        /// <summary>
        /// フキダシテキスト表示
        /// </summary>
        /// <param name="text"></param>
        protected override async void DisplayText(string text)
        {
            _fukidashiText.text = text;
            await ChangeFukidashiTransparencyAsync(1, 0.5f);
            cts = new();
            await UniTask.Delay(TimeSpan.FromSeconds(text.Length * TextSpeed * 2f), cancellationToken: cts.Token);
            await ChangeFukidashiTransparencyAsync(0, 0.5f);
            textInd.Value++;
            if (textInd.Value == allText.Count)
            {
                _textFinish.OnNext(Unit.Default);
                SetActive(false);
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(200), cancellationToken: cts.Token);
                DisplayText(allText.ElementAt(textInd.Value));
            }
        }

        /// <summary>
        /// テキスト送り
        /// </summary>
        public override void SkipText()
        {
            //if (isText) cts.Cancel();
        }

        /// <summary>
        /// テキストボックスアクティブ状態設定
        /// </summary>
        /// <param name="active"></param>
        public override async void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
            if (active)
            {
                await ChangeFukidashiTransparencyAsync(0, 0);
                DisplayText(allText.ElementAt(textInd.Value));
            }
        }

        public override void OnDestroy()
        {
            if (sequence.IsActive()) sequence.Kill();
        }

        /// <summary>
        /// フキダシ透明度変更
        /// </summary>
        /// <param name="alpha">透明度（0～1）</param>
        /// <param name="time">トゥイーン完了までの時間</param>
        /// <returns></returns>
        private async UniTask ChangeFukidashiTransparencyAsync(float alpha, float time)
        {
            sequence = DOTween.Sequence();
            await sequence.Append(_fukidashi.DOFade(alpha, time))
                .Join(_fukidashiText.DOFade(alpha, time));
        }
    }
}