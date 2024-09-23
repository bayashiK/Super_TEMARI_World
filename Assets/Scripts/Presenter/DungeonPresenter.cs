using System;
using System.Collections;
using TEMARI.Model;
using TEMARI.View;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

namespace TEMARI.Presenter
{
    public class DungeonPresenter : PresenterBase
    {
        [SerializeField] private View.CustomButton _startButton;

        [SerializeField] private View.CustomButton _attackButton;

        /// <summary>タイトルバックボタン</summary>
        [SerializeField] private View.CustomButton _backButton;

        /// <summary>ヘッダーマネージャー</summary>
        [SerializeField] private View.HeaderManager _headerManager;

        /// <summary>タイトルバック確認ダイアログ</summary>
        [SerializeField] private View.NoticeDialogue _noticeDialogue;

        [SerializeField] private SDBase _sd1;
        [SerializeField] private SDBase _sd2;

        protected override void Init()
        {
            base.Init();

            //タイトルバック確認ダイアログ表示
            _backButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => {
                    _noticeDialogue.OpenDialogue("ホーム画面に戻ります");
                    _noticeDialogue.OnButtonClicked
                        .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                        .Subscribe(async _ => await baseModel.ChangeSceneAsync("Home"))
                        .AddTo(this)
                        .AddTo(_noticeDialogue.cancellationToken);
                })
                .AddTo(this);

            _startButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => {
                    _sd1.StartMove();
                    _sd2.StartMove();
                })
                .AddTo(this);

            _attackButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => {
                    _sd1.Attack();
                })
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