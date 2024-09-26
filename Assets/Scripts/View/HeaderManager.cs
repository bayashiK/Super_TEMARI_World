using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.View
{
    /// <summary>
    /// ヘッダー管理クラス
    /// </summary>
    public class HeaderManager : MonoBehaviour
    {
        /// <summary>所持マニー表示テキスト</summary>
        [SerializeField] private Text _moneyText;

        /// <summary>満腹度表示テキスト</summary>
        [SerializeField] private Text _fullnessText;

        public void Init(string money, int fullness, float fullnessRatio)
        {
            _moneyText.text = money;

        }

        /// <summary>
        /// ヘッダーのコイン表示を更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async UniTask UpdateMoneyDisp(int value)
        {
            int previousValue;
            bool result = int.TryParse(_moneyText.text, NumberStyles.AllowThousands, null, out previousValue);
            if (result)
            {
                await _moneyText.DOCounter(previousValue, value, 0.5f, true);
            }
        }
    }
}