using System;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TEMARI.View {
    /// <summary>
    /// 通知ダイアログクラス
    /// </summary>
    public class NoticeDialogue : DialogueBase
    {
        /// <summary> ボタン </summary>
        protected CustomButton button;

        /// <summary>
        /// ボタンクリック時
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
