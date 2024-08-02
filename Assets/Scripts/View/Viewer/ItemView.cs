using System;
using System.Collections;
using TMPro;
using UniRx;
using UnityEngine;

namespace TEMARI.View
{
    /// <summary>
    /// アイテムビューアに表示するアイテムビュークラス
    /// </summary>
    public class ItemView : MonoBehaviour
    {
        /// <summary> アイテム名 </summary>
        [SerializeField] private TextMeshProUGUI _name;

        /// <summary> 説明文 </summary>
        [SerializeField] private TextMeshProUGUI _description;

        /// <summary> 所持数 </summary>
        [SerializeField] private TextMeshProUGUI _possession;

        /// <summary> ボタン </summary>
        [SerializeField] private CustomButton _button;
        private ScaledButtonView _scaledButton;

        /// <summary> ボタンクリック通知 </summary>
        public IObservable<Unit> OnButtonClicked => _button.OnButtonClicked;

        private void Awake()
        {
            _scaledButton = _button.GetComponent<ScaledButtonView>();
        }

        /// <summary>
        /// 表示内容セット
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="pssession"></param>
        public void SetDisp(string name, string description, int pssession, string buttonText)
        {
            SetItemName(name);
            SetItemDescription(description);
            SetItemPossession(pssession);
            SetItemButtonText(buttonText);
        }

        /// <summary>
        /// アイテム名設定
        /// </summary>
        /// <param name="name"></param>
        public void SetItemName(string name)
        {
            _name.text = name;
        }

        /// <summary>
        /// 説明文設定
        /// </summary>
        /// <param name="description"></param>
        public void SetItemDescription(string description)
        {
            _description.text = description;
        }

        /// <summary>
        /// 所持数設定
        /// </summary>
        /// <param name="possession"></param>
        public void SetItemPossession(int possession)
        {
            _possession.text = possession.ToString();
        }

        /// <summary>
        /// ボタンテキスト設定
        /// </summary>
        /// <param name="text"></param>
        public void SetItemButtonText(string text)
        {
            _scaledButton.SetButtonText(text);
        }

        public override string ToString()
        {
            return $"name:{_name.text}, description:{_description.text}, possession:{_possession.text}";
        }
    }
}