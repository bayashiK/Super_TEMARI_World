using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TEMARI.View
{
    /// <summary>
    /// 設定ダイアログクラス
    /// </summary>
    public class SettingDialogue : DialogueBase
    {
        /// <summary> テキスト表示スピード変更スライダー </summary>
        [SerializeField] protected Slider textSpeedSlider;
        /// <summary> テキスト表示スピードプレビュー </summary>
        [SerializeField] protected Text previewText;
        private string[] _testMessage = new string[] 
        {
            "ん、私ともあっちむいてホイをやるべき", "プリンを二つも食べちゃいます！", "あっつい…暑くて干からびそう…", "動いてないのに暑いよ～…", 
            "エッチなのはダメ！死刑！", "先生、ちょっとお時間いただけますか？", "アイス奢らなきゃいけなくなったじゃん！", "シュン、お前も寝ろ！",
            "セクシーセイアですまない…", "エビが消えた・・・？", "覚悟しろ！規則違反者共め！", 
            "感謝感謝ありがとありがと\n感謝感謝ありがと感謝感謝\nありがとありがと感謝感謝\nありがと感謝感謝ありがと\n感謝感謝感謝ありがと感謝\n" +
            "感謝ありがとありがと感謝\n感謝ありがと感謝感謝\nありがとありがと感謝感謝\nありがと感謝感謝ありがと\nありがと感謝感謝感謝感謝"
        };
        /// <summary> テキスト表示スピード変更時 </summary>
        public IObservable<float> OnTextSpeedChanged => textSpeedSlider
            .OnValueChangedAsObservable();

        /// <summary> BGM音量変更スライダー </summary>
        [SerializeField] protected Slider BGMVolumeSlider;
        /// <summary> BGM音量変更時 </summary>   
        public IObservable<float> OnBGMVolumeChanged => BGMVolumeSlider
            .OnValueChangedAsObservable();

        /// <summary> SE音量変更スライダー </summary>
        [SerializeField] protected Slider SEVolumeSlider;
        /// <summary> SE音量変更時 </summary>
        public IObservable<float> OnSEVolumeChanged => SEVolumeSlider
            .OnValueChangedAsObservable();

        protected override void Start()
        {
            base.Start();

            OnTextSpeedChanged
                .Subscribe(x => PreviewTextSpeed((int)x))
                .AddTo(this);
        }

        /// <summary>
        /// テキスト表示スピードのプレビュー
        /// </summary>
        /// <param name="ind"></param>
        private void PreviewTextSpeed(int ind)
        {
            if (!this.gameObject.activeSelf) return;
            previewText.text = "";
            if (DOTween.IsTweening(previewText))
            {
                previewText.DOKill();
            }
            var testMessage = _testMessage[UnityEngine.Random.Range(0, _testMessage.Length )];
            previewText.DOText(testMessage, testMessage.Length * DB.BasicData.TextSpeed[ind]).SetEase(Ease.Linear);
        }

        /// <summary>
        /// ダイアログのアクティブ状態設定（呼び出し時にテキストリセット）
        /// </summary>
        /// <param name="active"></param>
        public override void SetDialogueActive(bool active)
        {
            previewText.text = "";
            if (DOTween.IsTweening(previewText)) previewText.DOKill();
            base.SetDialogueActive(active);
        }

        /// <summary>
        /// スライダーの値セット
        /// </summary>
        /// <param name="speed"></param>
        public void SetSliderValue(int textSpeed, float BGMVolume, float SEVolume)
        {
            textSpeedSlider.value = textSpeed;
            BGMVolumeSlider.value = BGMVolume;
            SEVolumeSlider.value = SEVolume;
        }
    }
}