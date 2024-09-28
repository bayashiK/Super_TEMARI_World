using System;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using DG.Tweening;
using System.Collections.Generic;


namespace TEMARI.View {
    /// <summary>
    /// ��V�_�C�A���O�N���X
    /// </summary>
    public class RewordDialogue : DialogueBase
    {
        /// <summary> �{�^�� </summary>
        [SerializeField] protected CustomButton button;

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

            closeButton.SetActive(false);

            base.Start();
        }

        /// <summary>
        /// �_�C�A���O�I�[�v��
        /// </summary>
        /// <param name="reword"></param>
        public async void OpenDialogue(string reword)
        {
            SetMainText(reword + "\n<size=40>����肵�܂���</size>");
            this.gameObject.SetActive(true);
            dialogueRect.localScale = Vector2.zero;
            await dialogueRect.DOScale(originalScale, 0.15f);
        }

        /// <summary>
        /// �_�C�A���O�I�[�v��
        /// </summary>
        /// <param name="reword"></param>
        public void OpenDialogue(List<string> reword)
        {
            string str = string.Join("\n", reword);
            OpenDialogue(str);
        }
    }
}
