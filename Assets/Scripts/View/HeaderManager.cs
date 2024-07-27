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

        /// <summary>メーターの残量に応じた色指定</summary>
        private Color32[] _meterColor = { new Color32(150, 255, 100, 255), new Color32(255, 240, 90, 255), new Color32(240, 80, 80, 255) };

        public void Init(string money, int fullness, float fullnessRatio)
        {
            _moneyText.text = money;
            _ = UpdateFullnessMeter(fullness, fullnessRatio);
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

        /// <summary>
        /// 満腹度メーターの表示を更新
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public async UniTask UpdateFullnessMeter(int value, float ratio)
        {
            if (value == 0)
            {
                _fullnessText.color = new Color32(255, 0, 0, 255);
            }
            else
            { 
                _fullnessText.color = new Color32(50, 50, 50, 255); 
            }

            _fullnessMeter.color = ratio switch
            {
                > 0.5f => _meterColor[0],
                > 0.2f => _meterColor[1],
                _ => _meterColor[2]
            };

            int previousValue = int.Parse(_fullnessText.text);
            var sequence = DOTween.Sequence();
            await sequence.Append(_fullnessMeter.DOFillAmount(ratio, 0.5f))
                .Join(_fullnessText.DOCounter(previousValue, value, 0.5f, true));

        }
    }
}