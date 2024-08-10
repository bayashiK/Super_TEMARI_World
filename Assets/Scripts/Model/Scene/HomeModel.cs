using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TEMARI.Model
{
    /// <summary>
    /// ホーム画面モデル
    /// </summary>
    public class HomeModel : SceneBase
    {
        [SerializeField] private DB.ItemData _itemData;
        /// <summary> アイテムデータベース </summary>
        public DB.ItemData ItemData { get { return _itemData; } }

        [SerializeField] private DB.TextData _textData;
        /// <summary> テキストデータベース </summary>
        public DB.TextData TextData { get { return _textData; } }
    }
}