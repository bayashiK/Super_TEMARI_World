using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TEMARI.Model;
using DG.Tweening;

namespace TEMARI.Presenter
{
    /// <summary>
    /// ホーム画面プレゼンター
    /// </summary>
    public class HomePresenter : PresenterBase
    {
        /// <summary>ヘッダーマネージャー</summary>
        [SerializeField] private View.HeaderManager _headerManager;

        /// <summary>タイトルバックボタン</summary>
        [SerializeField] private View.CustomButton _backButton;

        /// <summary>タイトルバック確認ダイアログ</summary>
        [SerializeField] private View.NoticeDialogue _backDialogue;

        [SerializeField] private View.StatusWindowManager _statusWindowManager;

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

            baseModel.BasicData.Money = -1000;
            baseModel.BasicData.Fullness = 50;
            
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
                .Subscribe(_ => textManager.SetTextType(View.TextType.Talk, baseModel.AllText))
                .AddTo(this);

            _babanukiButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => { baseModel.BasicData.Fullness = -5; baseModel.BasicData.Money = 100; })
                .AddTo(this);

            //マニー所持数変化
            baseModel.BasicData.OnMoneyChanged
                .Subscribe(async x => await _headerManager.UpdateMoneyDisp(x))
                .AddTo(this);

            //満腹度変化通知
            baseModel.BasicData.OnFullnessChanged
                .Subscribe(async x => await _headerManager.UpdateFullnessMeter(x, x / (float)DB.BasicData.MaxFullness))
                .AddTo(this);

            //ヘッダーの表示初期化
            _headerManager.Init(baseModel.BasicData.Money.ToString("N0"), baseModel.BasicData.Fullness, baseModel.BasicData.Fullness / (float)DB.BasicData.MaxFullness);
        }
    }
}