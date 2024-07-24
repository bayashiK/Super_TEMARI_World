﻿using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;

namespace TEMARI.View
{
    /// <summary> テキスト種別 </summary>
    public enum TextType
    {
        Talk,
        Fukidashi,
        Help,
        None
    }

    /// <summary>
    /// テキスト管理Viewクラス
    /// </summary>
    public class TextManager : MonoBehaviour
    {
        /// <summary> 現在表示対象のテキスト種別 </summary>
        private TextType currentTextType;

        /// <summary> 会話テキストボックス </summary>
        [SerializeField] private TalkTextBox _talkTextBox;

        /// <summary> フキダシ </summary>
        [SerializeField] private Fukidashi _fukidashi;

        /// <summary> 全種類のテキストボックス </summary>
        private TextBoxBase[] _allTextBox;

        /// <summary> テキストを最後まで表示しきったか </summary>
        public IObservable<Unit> TextFinish => _textFinish;
        protected Subject<Unit> _textFinish = new();

        private void Awake()
        {
            _allTextBox = new TextBoxBase[] {_talkTextBox, _fukidashi};
            _textFinish.AddTo(this);
        }

        private void Start()
        {
            _talkTextBox.TextFinish
                .Subscribe(_ => _textFinish.OnNext(Unit.Default))
                .AddTo(this);

            _fukidashi.TextFinish
                .Subscribe(_ => _textFinish.OnNext(Unit.Default))
                .AddTo(this);
        }

        /// <summary>
        /// テキスト表示種別変更
        /// </summary>
        /// <param name="isEnabled"> 表示状態 </param>
        /// <param name="text"> 表示テキスト </param>
        public void SetTextBoxActive(TextType type, IReadOnlyCollection<string> text)
        {
            currentTextType = type;
            SetAllText(text);
            switch (type)
            {
                case TextType.Talk:
                    _talkTextBox.SetActive(true);
                    _fukidashi.SetActive(false);
                    break;
                case TextType.Fukidashi:
                    _talkTextBox.SetActive(false);
                    _fukidashi.SetActive(true);
                    break;
                default:
                    _talkTextBox.SetActive(false);
                    _fukidashi.SetActive(false);
                    break;
            }
        }

        /// <summary>
        /// 全てのテキストボックスにテキスト表示スピード反映
        /// </summary>
        /// <param name="speed"></param>
        public void SetTextSpeed(float speed)
        {
            foreach(var textBox in _allTextBox)
            {
                textBox.TextSpeed = speed;
            }
        }

        /// <summary>
        /// 画面クリック時のテキスト処理
        /// </summary>
        public void OnClicked()
        {
            GetCurrentTextBox()?.SkipText();
        }

        /// <summary>
        /// テキスト全文セット
        /// </summary>
        /// <param name="text"></param>
        public void SetAllText(IReadOnlyCollection<string> text)
        {
            GetCurrentTextBox()?.SetAllText(text);
        }

        /// <summary>
        /// 現在表示対象のテキスト種別に応じてテキストボックスを返す
        /// </summary>
        /// <returns></returns>
        private TextBoxBase GetCurrentTextBox()
        {
            return currentTextType switch
            {
                TextType.Talk => _talkTextBox,
                TextType.Fukidashi => _fukidashi,
                _ => null
            };
        }
    }
}