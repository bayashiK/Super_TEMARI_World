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
        private Color32 _defaultButtonColor;
        /// <summary>�I�����̃{�^���X�P�[���{��</summary>
        private float _pressedScale = 0.95f;
        /// <summary>�{�^���L�����̕s�����x</summary>
        private const byte ActiveImageAlpha = 255;
        /// <summary>�{�^���������̕s�����x</summary>
        private const byte InactiveImageAlpha = 127;

        private Vector2 _defaultScale;

        private CustomButton _button;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        private void Start()
        {
            _defaultButtonColor = _image.color;
            _defaultScale = this.transform.localScale;
            _button = GetComponent<CustomButton>();

            _button.OnButtonClicked.
                Subscribe(_ => SoundManager.Instance.PlaySE(SoundManager.SEType.Click))
                .AddTo(this);

            _button.OnButtonPressed
                .Subscribe(_ => SetScale(_pressedScale))
                .AddTo(this);

            _button.OnButtonReleased
                .Subscribe(_ => SetScale(1))
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
            this.transform.localScale = _defaultScale * scale;
            //_image.rectTransform.localScale = _defaultButtonScale * scale;
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
