using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TEMARI.DB
{
    [CreateAssetMenu(fileName = "TextData", menuName = "ScriptableObject/TextData")]
    public class TextData : ScriptableObject
    {
        /// <summary> テキストリスト </summary>
        public IReadOnlyCollection<ShowText> TextList => _textList;
        private List<ShowText> _textList = new()
        {
            new ShowText(0, TextTag.None,
                new List<(int, string)> { (0, "テストメッセージ\nテスト1テスト1テスト1"), (5, "テストメッセージ\nテスト2テスト2テスト2") }),
            new ShowText(1, TextTag.None, new List<(int, string)> { (0, "表情0") }),
            new ShowText(1, TextTag.None, new List<(int, string)> { (1, "表情1") }),
            new ShowText(1, TextTag.None, new List<(int, string)> { (2, "表情2") }),
            new ShowText(1, TextTag.None, new List<(int, string)> { (3, "表情3") }),
            new ShowText(1, TextTag.None, new List<(int, string)> { (4, "表情4") }),
            new ShowText(1, TextTag.None, new List<(int, string)> { (5, "表情5") }),
            new ShowText(1, TextTag.None, new List<(int, string)> { (6, "表情6") })
        };

        public void InitList()
        {
            _textList.Clear();
            _textList.Add(new ShowText(0, TextTag.None,
                new List<(int, string)> { (0, "テストメッセージ\nテスト1テスト1テスト1"), (5, "テストメッセージ\nテスト2テスト2テスト2") }));
            _textList.Add(new ShowText(1, TextTag.None, new List<(int, string)> { (0, "表情0") }));
            _textList.Add(new ShowText(1, TextTag.None, new List<(int, string)> { (1, "表情1") }));
            _textList.Add(new ShowText(1, TextTag.None, new List<(int, string)> { (2, "表情2") }));
            _textList.Add(new ShowText(1, TextTag.None, new List<(int, string)> { (3, "表情3") }));
            _textList.Add(new ShowText(1, TextTag.None, new List<(int, string)> { (4, "表情4") }));
            _textList.Add(new ShowText(1, TextTag.None, new List<(int, string)> { (5, "表情5") }));
            _textList.Add(new ShowText(1, TextTag.None, new List<(int, string)> { (6, "表情6") }));
        }

        /// <summary>
        /// 指定した種別、タグに合致するテキストを返す(複数該当する場合はランダム)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public ShowText GetShowText(int type, TextTag tag)
        {
            var list = _textList.Where(x => x.Type == type && x.Tag == tag);
            if(list.Count() > 1)
            {
                int n = Random.Range(0, list.Count());
                return list.ElementAt(n);
            }
            else
            {
                return list.ElementAt(0);
            }
        }
    }

    /// <summary> テキスト検索タグ </summary>
    public enum TextTag
    {
        None
    }

    /// <summary>
    /// 表示テキストクラス
    /// </summary>
    public class ShowText
    {
        private int _type;
        /// <summary> テキストタイプ　0:talk, 1:fukidashi, 2:help </summary>
        public int Type
        {
            get { return _type; }
        }

        private TextTag _tag;
        /// <summary> テキスト検索タグ </summary>
        public TextTag Tag
        {
            get { return _tag; }
        }

        private List<(int face, string text)> _allText;
        /// <summary> テキスト全文  表情種別　0:normal, 1:fear, 2:jitome, 3:kirakira, 4:sad, 5:smile, 6:tere</summary>
        public IReadOnlyCollection<(int face, string text)> AllText
        {
            get { return _allText; }
        }

        public ShowText(int type, TextTag tag, List<(int face, string text)> allText)
        {
            _type = type;
            _tag = tag;
            _allText = allText;
        }
    }
}