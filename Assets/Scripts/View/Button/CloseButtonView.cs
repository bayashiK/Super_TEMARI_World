using System.Drawing;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.View
{
    /// <summary>
    /// カーソルを合わせると変色するクローズボタンView
    /// </summary>
    [RequireComponent(typeof(CustomButton))]
    public class CloseButtonView : MonoBehaviour
    {
        /// <summary>デフォルトのボタンカラー</summary>
        private Color32 _defaultButtonColor = new Color32(255, 255, 255, 0);
        /// <summary>カーソルを合わせた際のボタンカラー</summary>
        private readonly Color32 _enterdButtonColor = new Color32(255, 70, 70, 255);

        /// <summary>デフォルトの文字色</summary>
        private Color32 _defaultTextColor = new Color32(55, 55, 55, 255);
        /// <summary>カーソルを合わせた際の文字色</summary>
        private readonly Color32 _enterdTextColor = new Color32(255, 255, 255, 255);

        private CustomButton _button;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        private void Start()
        {
            _defaultButtonColor = _image.color;
            _defaultTextColor = _text.color;
            _button = GetComponent<CustomButton>();

            _button.OnButtonEntered
                .Subscribe(_ => SetColor(_enterdButtonColor, _enterdTextColor))
                .AddTo(this);

            _button.OnButtonExited
                .Subscribe(_ => SetColor(_defaultButtonColor, _defaultTextColor))
                .AddTo(this);
        }

        /// <summary>
        /// ボタンのカラー変更
        /// </summary>
        /// <param name="color"></param>
        private void SetColor(Color32 buttonColor, Color32 textColor)
        {
            _image.color = buttonColor;
            _text.color = textColor;
        }

        /// <summary>
        /// ボタンをデフォルトカラーに戻す
        /// </summary>
        public void SetDefaultColor()
        {
            _image.color = _defaultButtonColor;
            _text.color = _defaultTextColor;
        }
    }
}