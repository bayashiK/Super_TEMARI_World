using System;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEMARI.View {
    /// <summary>
    /// �ʒm�_�C�A���O�N���X
    /// </summary>
    public class NoticeDialogue : DialogueBase
    {
        /// <summary> �{�^�� </summary>
        protected CustomButton button;

        /// <summary>
        /// �{�^���N���b�N��
        /// </summary>
        public IObservable<Unit> OnButtonClicked => button.OnButtonClicked;

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            button = dialogue.transform.Find("Button").GetComponent<CustomButton>();
        }
    }
}
