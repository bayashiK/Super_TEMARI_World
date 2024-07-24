using DG.Tweening;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Linq;
using Cysharp.Threading.Tasks;
using System;

namespace TEMARI.View
{
    /// <summary>
    /// 会話テキストボックスクラス
    /// </summary>
    public class TalkTextBox : TextBoxBase
    {

        /// <summary> 会話キャラ名 </summary>
        private TextMeshProUGUI _charaName;

        /// <summary> 会話テキスト </summary>
        private Text _talkText;

        /// <summary> テキスト表示完了した際の▼マーク </summary>
        private Text _nextText;

        protected override void Awake()
        {
            _charaName = transform.Find("CharaName").GetComponent<TextMeshProUGUI>();
            _talkText = transform.Find("Text").GetComponent<Text>();
            _nextText = transform.Find("Next").GetComponent<Text>();
            base.Awake();
        }

        /// <summary>
        /// 会話テキスト表示
        /// </summary>
        /// <param name="text"></param>
        protected override async void DisplayText(string text)
        {
            _talkText.text = "";
            textInd++;
            await _talkText.DOText(text, text.Length * TextSpeed).SetEase(Ease.Linear);
            _ = _nextText.DOFade(1, 0.4f).SetLoops(-1, LoopType.Yoyo);
        }

        /// <summary>
        /// テキスト送り
        /// </summary>
        public override void SkipText()
        {
            if (!isText) return;

            if (DOTween.IsTweening(_talkText)) //テキスト送り中
            {
                _talkText.DOComplete();
                _nextText.DOFade(1, 0.4f).SetLoops(-1, LoopType.Yoyo); //▼を点滅させる
            }
            else
            {
                if (DOTween.IsTweening(_nextText))
                {
                    _nextText.DOKill();
                    _nextText.DOFade(0, 0);
                }
                
                if (IsTextFinish())
                {
                    _textFinish.OnNext(Unit.Default);
                    isText = false;
                    return;
                }
                DisplayText(allText.ElementAt(textInd));
            }
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
                _talkText.text = "";
                await _nextText.DOFade(0, 0);
                isText = false;
                this.rectTransform.localScale = Vector2.zero;
                await this.rectTransform.DOScale(1, 0.1f);
                await UniTask.Delay(TimeSpan.FromMilliseconds(200));
                isText = true;
                DisplayText(allText.ElementAt(textInd));
            }
        }

        /// <summary>
        /// テキストを最後まで表示しきっているならtrueを返す
        /// </summary>
        /// <returns></returns>
        public override bool IsTextFinish()
        {
            return (textInd == allText.Count);
        }

        public override void OnDestroy()
        {
            if (DOTween.IsTweening(_talkText))
            {
                _talkText.DOComplete();
            }
            if (DOTween.IsTweening(_nextText))
            {
                _nextText.DOKill();
            }
        }
    }
}