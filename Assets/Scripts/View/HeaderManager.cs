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

        /// <summary>満腹度メーター</summary>
        [SerializeField] private Image _fullnessMeter;

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

        /// <summary>
        /// 満腹度メーターの表示を更新
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public async UniTask UpdateFullnessMeter(int value, float ratio)
        {
            int previousValue = int.Parse(_fullnessText.text);
            var sequence = DOTween.Sequence();
            await sequence.Append(_fullnessMeter.DOFillAmount(ratio, 0.5f))
                .Join(_fullnessText.DOCounter(previousValue, value, 0.5f, true));
        }
    }
}