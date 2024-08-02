using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TEMARI.Model
{
    /// <summary>
    /// ホーム画面モデル
    /// </summary>
    public class HomeModel : SceneBase
    {
        /// <summary> アイテムデータベース </summary>
        [SerializeField] protected DB.ItemData itemData;
        /// <summary> アイテムデータベース </summary>
        public DB.ItemData ItemData { get { return itemData; } }
    }
}