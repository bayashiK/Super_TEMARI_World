using TEMARI.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.View
{
    /// <summary>
    /// �I������Ɗg�k����{�^��View
    /// </summary>
    [RequireComponent(typeof(CustomButton))]
    public class ScaledButtonView : MonoBehaviour
    {
        private Vector3 _defaultButtonScale;
        private Color32 _defaultButtonColor;
        /// <summary>�f�t�H���g�̃{�^���X�P�[���{��</summary>
        private const float DefaultScale = 1f;
        /// <summary>�I�����̃{�^���X�P�[���{��</summary>
        private const float PressedScale = 0.95f;
        /// <summary>�{�^���L�����̕s�����x</summary>
        private const byte ActiveImageAlpha = 255;
        /// <summary>�{�^���������̕s�����x</summary>
        private const byte InactiveImageAlpha = 127;

        private CustomButton _button;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        private void Start()
        {
            _defaultButtonScale = _image.rectTransform.localScale;
            _defaultButtonColor = _image.color;
            _button = GetComponent<CustomButton>();

            _button.OnButtonClicked.
                Subscribe(_ => SoundManager.Instance.PlaySE(SoundManager.SEType.Click))
                .AddTo(this);

            _button.OnButtonPressed
                .Subscribe(_ => SetScale(PressedScale))
                .AddTo(this);

            _button.OnButtonReleased
                .Subscribe(_ => SetScale(DefaultScale))
                .AddTo(this);

            _button.IsActiveRP
                .Subscribe(SetButtonActive)
                .AddTo(this);
        }

        /// <summary>
        /// �{�^���̃X�P�[���ύX
        /// </summary>
        /// <param name="scale"></param>
        private void SetScale(float scale)
        {
            _image.rectTransform.localScale = _defaultButtonScale * scale;
            //_image.rectTransform.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// �{�^���̃A�N�e�B�u��Ԑ؂�ւ�
        /// </summary>
        /// <param name="isActive"></param>
        private void SetButtonActive(bool isActive)
        {
            var alpha = isActive ? ActiveImageAlpha : InactiveImageAlpha;
            _image.color = new Color32(_defaultButtonColor.r, _defaultButtonColor.g, _defaultButtonColor.b, alpha);
        }

        /// <summary>
        /// �{�^���̃e�L�X�g�Z�b�g
        /// </summary>
        /// <param name="text"></param>
        public void SetButtonText(string text)
        {
            _text.text = text;
        }
    }
}
