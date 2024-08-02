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
        [SerializeField] protected TextMeshProUGUI caption;
        /// <summary> ダイアログ本文 </summary>
        [SerializeField] protected TextMeshProUGUI mainText;

        /// <summary> 閉じるボタン </summary>
        [SerializeField] protected CustomButton closeButton;
        protected CloseButtonView closeButtonView;

        /// <summary> 元スケール </summary>
        protected float originalScale;

        /// <summary> ダイアログを閉じるイベント </summary>
        protected IObservable<Unit> dialogueClosed => Observable.Merge(closeButton.OnButtonClicked, bgMouseManager.OnClicked);

        protected virtual void Awake()
        {
            dialogueRect = dialogue.GetComponent<RectTransform>();
            closeButtonView = closeButton.GetComponent<CloseButtonView>();
        }

        protected virtual void Start()
        {
            originalScale = dialogueRect.localScale.x;

            //閉じるボタンクリック時ダイアログ非表示
            dialogueClosed
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => CloseDialogue().Forget())
                .AddTo(this);

            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// ダイアログオープン
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask OpenDialogue()
        {
            SoundManager.Instance.PlaySE(SoundManager.SEType.PopUp);
            this.gameObject.SetActive(true);
            closeButtonView.SetDefaultColor();
            dialogueRect.localScale = Vector2.zero;
            await dialogueRect.DOScale(originalScale, 0.15f).SetEase(Ease.OutBack, 1);
        }

        /// <summary>
        /// ダイアログクローズ
        /// </summary>
        /// <returns></returns>
        public virtual async UniTask CloseDialogue()
        {
            SoundManager.Instance.PlaySE(SoundManager.SEType.Cancel);
            await dialogueRect.DOScale(0, 0.15f).SetEase(Ease.Linear);
            this.gameObject.SetActive(false);
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