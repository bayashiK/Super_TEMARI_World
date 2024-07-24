using System;
using UniRx;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;
using TEMARI.Model;

namespace TEMARI.View
{
    /// <summary>
    /// ダイアログベースクラス
    /// </summary>
    public class DialogueBase : MonoBehaviour
    {
        /// <summary> グレーアウト用背景 </summary>
        [SerializeField] protected BGMouseManager bgMouseManager;

        /// <summary> ダイアログ本体 </summary>
        [SerializeField] protected GameObject dialogue;
        /// <summary> ダイアログ自体のRectTransform </summary>
        protected RectTransform dialogueRect;

        /// <summary> ダイアログ見出し </summary>
        protected TextMeshProUGUI caption;
        /// <summary> ダイアログ本文 </summary>
        protected TextMeshProUGUI mainText;

        /// <summary> 閉じるボタン </summary>
        protected CustomButton closeButton;
        protected CloseButtonView closeButtonView;

        /// <summary> ダイアログを閉じるイベント </summary>
        public IObservable<Unit> CloseDialogue => Observable.Merge(closeButton.OnButtonClicked, bgMouseManager.OnClicked);

        protected virtual void Awake()
        {
            dialogueRect = dialogue.GetComponent<RectTransform>();
            closeButton = dialogue.transform.Find("CloseButton").GetComponent<CustomButton>();
            caption = dialogue.transform.Find("Caption").GetComponent<TextMeshProUGUI>();
            mainText = dialogue.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            closeButtonView = closeButton.GetComponent<CloseButtonView>();
        }

        protected virtual void Start()
        {
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// ダイアログのアクティブ状態設定
        /// </summary>
        /// <param name="active"></param>
        public virtual async void SetDialogueActive(bool active)
        {
            if (active)
            {
                SoundManager.Instance.PlaySE(SoundManager.SEType.PopUp);
                this.gameObject.SetActive(active);
                closeButtonView.SetDefaultColor();
                dialogueRect.localScale = Vector2.zero;
                await dialogueRect.DOScale(1, 0.15f).SetEase(Ease.OutBack, 1);
            }
            else
            {
                SoundManager.Instance.PlaySE(SoundManager.SEType.Cancel);
                await dialogueRect.DOScale(0, 0.15f).SetEase(Ease.Linear);
                this.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// ダイアログ本文セット
        /// </summary>
        /// <param name="text"></param>
        public void SetMainText(string text)
        {
            mainText.text = text;
        }

        /// <summary>
        /// ダイアログ見出しセット
        /// </summary>
        /// <param name="text"></param>
        public void SetCaption(string text)
        {
            caption.text = text;
        }
    }
}