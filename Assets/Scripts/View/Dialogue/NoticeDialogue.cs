using System;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace TEMARI.View {
    /// <summary>
    /// �ʒm�_�C�A���O�N���X
    /// </summary>
    public class NoticeDialogue : DialogueBase
    {
        /// <summary> �{�^�� </summary>
        [SerializeField] protected CustomButton button;

        private CancellationTokenSource cancellationTokenSource;
        public CancellationToken cancellationToken { get; private set; }

        /// <summary>
        /// �{�^���N���b�N��
        /// </summary>
        public IObservable<Unit> OnButtonClicked => button.OnButtonClicked;

        protected override void Start()
        {
            //�{�^���N���b�N���_�C�A���O��\��
            button.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => CloseDialogue().Forget())
                .AddTo(this);

            base.Start();
        }

        /// <summary>
        /// �_�C�A���O�I�[�v��
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
        /// �_�C�A���O�N���[�Y
        /// </summary>
        /// <returns></returns>
        public override async UniTask CloseDialogue()
        {
            cancellationTokenSource.Cancel();
            await base.CloseDialogue();
        }
    }
}
