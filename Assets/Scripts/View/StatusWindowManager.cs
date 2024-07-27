using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.View {

    /// <summary>
    /// �X�e�[�^�X�E�B���h�E�Ǘ��N���X
    /// </summary>
    public class StatusWindowManager : MonoBehaviour
    {
        /// <summary>�e���x�\���e�L�X�g</summary>
        [SerializeField] private Text _affinityText;

        /// <summary>�̏d�\���e�L�X�g</summary>
        [SerializeField] private Text _weightText;

        /// <summary>�����͕\���e�L�X�g</summary>
        [SerializeField] private Text _attackText;

        /// <summary>�����^���\���e�L�X�g</summary>
        [SerializeField] private Text _mentalText;

        public void Init(string affinity, string weight, string attack, string mental)
        {
            _affinityText.text = affinity;
            _weightText.text = weight;
            _attackText.text = attack;
            _mentalText.text = mental;
        }

        /// <summary>
        /// �e���x�\���X�V
        /// </summary>
        /// <param name="value"></param>
        public void UpdateAffinityDisp(int value)
        {
            CountingText(_affinityText, value);
        }

        /// <summary>
        /// �̏d�\���X�V
        /// </summary>
        /// <param name="value"></param>
        public void UpdateWeightDisp(int value)
        {
            CountingText(_weightText, value);
        }

        /// <summary>
        /// �����͕\���X�V
        /// </summary>
        /// <param name="value"></param>
        public void UpdateAttackyDisp(int value)
        {
            CountingText(_attackText, value);
        }

        /// <summary>
        /// �����^���\���X�V
        /// </summary>
        /// <param name="value"></param>
        public void UpdateMentalDisp(int value)
        {
            CountingText(_mentalText, value);
        }

        /// <summary>
        /// �e�L�X�g�J�E���g�A�b�v����
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
