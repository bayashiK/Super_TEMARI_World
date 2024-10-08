﻿using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TEMARI.DB;

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
            "――足を引っ張ったら、殺すから。", "くっ……！『足を引っ張るなよ雑魚が』って？", "ま、また脅す気？\n簡単に従うと思わないで！",
            "……うっ……うっ……\n痛くて歩けない……おぶってぇ……","5キロも増えてませんっ！！！", "なっ……な、ななな…………ッお……\nお母さんぶらないでくれる！？",
            "（私の魂はカツ丼に惹かれている\n　それは、間違いない……）", "ああああああ！そんなことよりっ！\nシャワールームで毛虫見たよ！",
            "早く実力つけろ。できないならやめろ。", "まあ、過ぎたことはいいよ", "スマホ、没収されちゃった。", "……うわ、また落ちこぼれたちが変なことしてる",
            "留年してるんでしょう？劣等生だから", "りーぴゃん……なんで嫌そうな顔するの",
            "それって、もしかして……\n私がクールすぎるってこと？\nよく言われる。感情が薄くて、冷たい印象が作り物めいてるって。\n実力が安定しすぎるっていうのかな？\nそういうことじゃない？"
        };
        /// <summary> テキスト表示スピード変更時 </summary>
        public IObservable<float> OnTextSpeedChanged => textSpeedSlider
            .OnValueChangedAsObservable().Skip(1);  //初期化時の値(0)は通知しない

        /// <summary> BGM音量変更スライダー </summary>
        [SerializeField] protected Slider BGMVolumeSlider;
        /// <summary> BGM音量変更時 </summary>   
        public IObservable<float> OnBGMVolumeChanged => BGMVolumeSlider
            .OnValueChangedAsObservable().Skip(1);  //初期化時の値(0)は通知しない

        /// <summary> SE音量変更スライダー </summary>
        [SerializeField] protected Slider SEVolumeSlider;
        /// <summary> SE音量変更時 </summary>
        public IObservable<float> OnSEVolumeChanged => SEVolumeSlider
            .OnValueChangedAsObservable().Skip(1);  //初期化時の値(0)は通知しない

        /// <summary> 基本データベース </summary>
        [SerializeField] private DB.BasicData _basicData;

        protected override void Start()
        {
            SetSliderValue();

            OnTextSpeedChanged
                .Subscribe(x => PreviewTextSpeed((int)x))
                .AddTo(this);

            base.Start();
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
        /// ダイアログオープン（呼び出し時にテキストリセット）
        /// </summary>
        /// <param name="active"></param>
        public override async UniTask OpenDialogue()
        {
            previewText.text = "";
            if (DOTween.IsTweening(previewText)) previewText.DOKill();
            await base.OpenDialogue();
        }

        /// <summary>
        /// スライダーの値セット
        /// </summary>
        /// <param name="speed"></param>
        private void SetSliderValue()
        {
            textSpeedSlider.value = _basicData.TextSpeedInt;
            BGMVolumeSlider.value = _basicData.BGMVolume;
            SEVolumeSlider.value = _basicData.SEVolume;
        }
    }
}