﻿using UnityEngine;
using System;
using System.Collections.Generic;
using UniRx;
using TEMARI.DB;
using TEMARI.Model;

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
        /// <summary> 基本データベース </summary>
        [SerializeField] private DB.BasicData _basicData;

        /// <summary> テキストデータベース </summary>
        [SerializeField] private DB.TextData _textData;

        /// <summary> 現在表示対象のテキスト種別 </summary>
        public TextType CurrentTextType { get; private set; } = TextType.None;

        /// <summary> 会話テキストボックス </summary>
        [SerializeField] private TalkTextBox _talkTextBox;

        /// <summary> フキダシ </summary>
        [SerializeField] private Fukidashi _fukidashi;

        /// <summary> 全種類のテキストボックス </summary>
        private TextBoxBase[] _allTextBox;

        /// <summary>キャラクターマネージャー</summary>
        [SerializeField] private CharacterManager _characterManager;

        /// <summary>表示する表情のリスト</summary>
        private List<int> _faceList = new();

        private void Awake()
        {
            _allTextBox = new TextBoxBase[] {_talkTextBox, _fukidashi};
        }

        private void Start()
        {
            SetTextSpeed();

            _talkTextBox.TextFinish
                .Subscribe(_ => CurrentTextType = TextType.None)
                .AddTo(this);

            _fukidashi.TextFinish
                .Subscribe(_ => CurrentTextType = TextType.None)
                .AddTo(this);

            _talkTextBox.TextInd
                .Where(x => x >= 0 && x < _faceList.Count)
                .Subscribe(x => _characterManager.ChangeFace((Face)_faceList[x]))
                .AddTo(this);

            _fukidashi.TextInd
                .Where(x => x >= 0 && x < _faceList.Count)
                .Subscribe(x => _characterManager.ChangeFace((Face)_faceList[x]))
                .AddTo(this);
        }

        /// <summary>
        /// テキスト表示種別変更
        /// </summary>
        /// <param name="isEnabled"> 表示状態 </param>
        /// <param name="text"> 表示テキスト </param>
        public void SetTextType(TextType type)
        {
            if (CurrentTextType == TextType.None)
            {
                var showText = _textData.GetShowText((int)type, DB.TextTag.None);
                _faceList.Clear();
                var textList = new List<string>();
                foreach(var text in showText.AllText)
                {
                    textList.Add(text.text);
                    _faceList.Add(text.face);
                }
                CurrentTextType = type;
                SetAllText(textList);
                SetTextBoxActive(type);
            }
        }

        /// <summary>
        /// テキスト表示スピード変更
        /// </summary>
        /// <param name="ind"></param>
        public void ChangeTextSpeed(int ind)
        {
            _basicData.TextSpeedInt = ind;
            SetTextSpeed();
        }

        /// <summary>
        /// テキストボックス表示状態変更
        /// </summary>
        /// <param name="type"></param>
        private void SetTextBoxActive(TextType type)
        {
            GetCurrentTextBox()?.SetActive(true);
        }

        /// <summary>
        /// 画面クリック時のテキスト処理
        /// </summary>
        public void OnClicked()
        {
            if (CurrentTextType != TextType.None)
            {
                GetCurrentTextBox()?.SkipText();
            }
        }

        /// <summary>
        /// 全てのテキストボックスにテキスト表示スピード反映
        /// </summary>
        /// <param name="speed"></param>
        private void SetTextSpeed()
        {
            foreach (var textBox in _allTextBox)
            {
                textBox.TextSpeed = _basicData.GetTextSpeedValue();
            }
        }

        /// <summary>
        /// テキスト全文セット
        /// </summary>
        /// <param name="text"></param>
        private void SetAllText(IReadOnlyCollection<string> text)
        {
            GetCurrentTextBox()?.SetAllText(text);
        }

        /// <summary>
        /// 現在表示対象のテキスト種別に応じてテキストボックスを返す
        /// </summary>
        /// <returns></returns>
        private TextBoxBase GetCurrentTextBox()
        {
            return CurrentTextType switch
            {
                TextType.Talk => _talkTextBox,
                TextType.Fukidashi => _fukidashi,
                _ => null
            };
        }
    }
}