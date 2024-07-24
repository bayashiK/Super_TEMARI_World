using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using TEMARI.Model;

namespace TEMARI.Presenter
{
    public class HomePresenter : PresenterBase
    {
        [SerializeField] private TextMeshProUGUI _coinText;

        /// <summary>タイトルバックボタン</summary>
        [SerializeField] private View.CustomButton _backButton;
        /// <summary>タイトルバック確認ダイアログ</summary>
        [SerializeField] private View.NoticeDialogue _backDialogue;

        [SerializeField] private View.CustomButton _endButton;

        [SerializeField] private View.CustomButton _babanukiButton;

        protected override void Start()
        {
            SoundManager.Instance.PlayBGM(SoundManager.BGMType.Home).Forget();
            base.Start();
        }

        protected override void Init()
        {
            base.Init();
            
            //タイトルバック確認ダイアログ表示
            _backButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => _backDialogue.SetDialogueActive(true))
                .AddTo(this);

            //タイトルへ戻る
            _backDialogue.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(async _ => await baseModel.ChangeSceneAsync("Title"))
                .AddTo(this);

            //タイトルバック確認ダイアログ非表示
            _backDialogue.CloseDialogue
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => _backDialogue.SetDialogueActive(false))
                .AddTo(this);

            //テキスト種別設定
            _endButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => baseModel.SetTextType(View.TextType.Talk))
                .AddTo(this);

            _babanukiButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(async _ => await baseModel.ChangeSceneAsync("Babanuki"))
                .AddTo(this);

            //コイン所持数変化
            baseModel.BasicData.OnCoinChanged
                .Subscribe(x => _coinText.text = x.ToString("N0"))
                .AddTo(this);
        }
    }
}