using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System.Collections.Generic;


namespace TEMARI.View {
    /// <summary>
    /// 報酬ダイアログクラス
    /// </summary>
    public class RewordDialogue : DialogueBase
    {
        /// <summary> ボタン </summary>
        [SerializeField] protected CustomButton button;

        /// <summary>
        /// ボタンクリック時
        /// </summary>
        public IObservable<Unit> OnButtonClicked => button.OnButtonClicked;

        protected override void Start()
        {
            //ボタンクリック時ダイアログ非表示
            button.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => CloseDialogue().Forget())
                .AddTo(this);

            closeButton.SetActive(false);

            base.Start();
        }

        /// <summary>
        /// ダイアログオープン
        /// </summary>
        /// <param name="reword"></param>
        public async void OpenDialogue(string reword)
        {
            SetMainText(reword + "\n<size=40>を入手しました</size>");
            this.gameObject.SetActive(true);
            dialogueRect.localScale = Vector2.zero;
            await dialogueRect.DOScale(originalScale, 0.15f);
        }

        /// <summary>
        /// ダイアログオープン
        /// </summary>
        /// <param name="reword"></param>
        public void OpenDialogue(List<string> reword)
        {
            string str = string.Join("\n", reword);
            OpenDialogue(str);
        }
    }
}
