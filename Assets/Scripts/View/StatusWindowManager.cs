using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.View {

    /// <summary>
    /// ステータスウィンドウ管理クラス
    /// </summary>
    public class StatusWindowManager : MonoBehaviour
    {
        /// <summary>親愛度表示テキスト</summary>
        [SerializeField] private Text _affinityText;

        /// <summary>体重表示テキスト</summary>
        [SerializeField] private Text _weightText;

        /// <summary>口撃力表示テキスト</summary>
        [SerializeField] private Text _attackText;

        /// <summary>メンタル表示テキスト</summary>
        [SerializeField] private Text _mentalText;

        public void Init(string affinity, string weight, string attack, string mental)
        {
            _affinityText.text = affinity;
            _weightText.text = weight;
            _attackText.text = attack;
            _mentalText.text = mental;
        }

        /// <summary>
        /// 親愛度表示更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateAffinityDisp(int value)
        {
            CountingText(_affinityText, value);
        }

        /// <summary>
        /// 体重表示更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateWeightDisp(int value)
        {
            CountingText(_weightText, value);
        }

        /// <summary>
        /// 口撃力表示更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateAttackyDisp(int value)
        {
            CountingText(_attackText, value);
        }

        /// <summary>
        /// メンタル表示更新
        /// </summary>
        /// <param name="value"></param>
        public void UpdateMentalDisp(int value)
        {
            CountingText(_mentalText, value);
        }

        /// <summary>
        /// テキストカウントアップ処理
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        private async void CountingText(Text text, int value)
        {
            var previousValue = int.Parse(text.text);
            await text.DOCounter(previousValue, value, 0.5f, true);
        }
    }
}
