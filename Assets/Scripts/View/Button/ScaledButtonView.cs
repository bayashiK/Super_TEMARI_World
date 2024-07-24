using TEMARI.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.View
{
    /// <summary>
    /// 選択すると拡縮するボタンView
    /// </summary>
    [RequireComponent(typeof(CustomButton))]
    public class ScaledButtonView : MonoBehaviour
    {
        private Vector3 _defaultButtonScale;
        private Color32 _defaultButtonColor;
        /// <summary>デフォルトのボタンスケール倍率</summary>
        private const float DefaultScale = 1f;
        /// <summary>選択時のボタンスケール倍率</summary>
        private const float PressedScale = 0.95f;
        /// <summary>ボタン有効時の不透明度</summary>
        private const byte ActiveImageAlpha = 255;
        /// <summary>ボタン無効時の不透明度</summary>
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
        /// ボタンのスケール変更
        /// </summary>
        /// <param name="scale"></param>
        private void SetScale(float scale)
        {
            _image.rectTransform.localScale = _defaultButtonScale * scale;
            //_image.rectTransform.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// ボタンのアクティブ状態切り替え
        /// </summary>
        /// <param name="isActive"></param>
        private void SetButtonActive(bool isActive)
        {
            var alpha = isActive ? ActiveImageAlpha : InactiveImageAlpha;
            _image.color = new Color32(_defaultButtonColor.r, _defaultButtonColor.g, _defaultButtonColor.b, alpha);
        }

        /// <summary>
        /// ボタンのテキストセット
        /// </summary>
        /// <param name="text"></param>
        public void SetButtonText(string text)
        {
            _text.text = text;
        }
    }
}
