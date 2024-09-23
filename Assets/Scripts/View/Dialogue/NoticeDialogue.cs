using System;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TEMARI.View {
    /// <summary>
    /// 通知ダイアログクラス
    /// </summary>
    public class NoticeDialogue : DialogueBase
    {
        /// <summary> ボタン </summary>
        [SerializeField] protected CustomButton button;

        private CancellationTokenSource cancellationTokenSource;
        public CancellationToken cancellationToken { get; private set; }

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

            base.Start();
        }

        /// <summary>
        /// ダイアログオープン
        /// </summary>
        /// <param name="mainText"></param>
        public async void OpenDialogue(string mainText)
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            SetMainText(mainText);
            await base.OpenDialogue();
        }

        /// <summary>
        /// ダイアログクローズ
        /// </summary>
        /// <returns></returns>
        public override async UniTask CloseDialogue()
        {
            cancellationTokenSource.Cancel();
            await base.CloseDialogue();
        }
    }
}
