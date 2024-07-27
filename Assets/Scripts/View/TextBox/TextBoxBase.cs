using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace TEMARI.View
{
    /// <summary>
    /// テキストボックスベースクラス
    /// </summary>
    public abstract class TextBoxBase : MonoBehaviour
    {
        /// <summary> テキストボックス自体のRectTransform </summary>
        protected RectTransform rectTransform;

        /// <summary> テキスト全文 </summary>
        protected IReadOnlyCollection<string> allText;

        /// <summary> テキスト表示インデックス </summary>
        protected int textInd = 0;

        /// <summary> テキスト表示スピード </summary>
        public float TextSpeed { get; set; }

        /// <summary> テキストを最後まで表示しきったか </summary>
        public IObservable<Unit> TextFinish => _textFinish;
        protected Subject<Unit> _textFinish = new();

        /// <summary> 会話テキストを表示可能か </summary>
        protected bool isText = false;

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// テキスト表示
        /// </summary>
        /// <param name="text"></param>
        abstract protected void DisplayText(string text);

        /// <summary>
        /// テキスト送り
        /// </summary>
        abstract public void SkipText();

        /// <summary>
        /// テキストボックスアクティブ状態設定
        /// </summary>
        /// <param name="active"></param>
        abstract public void SetActive(bool active);

        abstract public void OnDestroy();

        /// <summary>
        /// 表示テキスト全文セット
        /// </summary>
        /// <param name="text"></param>
        public void SetAllText(IReadOnlyCollection<string> text)
        {
            allText = text;
            textInd = 0;
        }
    }
}