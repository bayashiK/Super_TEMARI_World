﻿using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using TEMARI.Model;
using DG.Tweening;
using TEMARI.View;

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
        [SerializeField] private View.NoticeDialogue _noticeDialogue;

        /// <summary>キャラクターマネージャー</summary>
        [SerializeField] private CharacterManager _characterManager;

        /// <summary>ステータスウィンドウ</summary>
        //[SerializeField] private View.StatusWindowManager _statusWindowManager;

        /// <summary>アイテムビューア</summary>
        [SerializeField] private View.ItemViewer _itemViewer;

        [SerializeField] private View.CustomButton _endButton;

        [SerializeField] private View.CustomButton _babanukiButton;

        [SerializeField] private View.CustomButton _itemButton;

        [SerializeField] private View.CustomButton _gearButton;

        protected override void Start()
        {
            SoundManager.Instance.PlayBGM(SoundManager.BGMType.Home).Forget();
            base.Start();
        }

        protected override void Init()
        {
            base.Init();

            var homeModel = (Model.HomeModel)baseModel;
            homeModel.ItemData.InitList();
            //homeModel.TextData.InitList();
            baseModel.BasicData.Money = 0;
            baseModel.BasicData.Fullness = 50;

            //タイトルバック確認ダイアログ表示
            _backButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => {
                    _noticeDialogue.OpenDialogue("タイトルに戻ります");
                    _noticeDialogue.OnButtonClicked
                        .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                        .Subscribe(async _ => await baseModel.ChangeSceneAsync("Title"))
                        .AddTo(this)
                        .AddTo(_noticeDialogue.cancellationToken);
                })
                .AddTo(this);

            //テキスト種別設定
            _endButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => {
                    var showText = homeModel.TextData.GetShowText(0, DB.TextTag.None);
                    textManager.SetTextType(showText);
                })
                .AddTo(this);

            //キャラクタークリック時
            _characterManager.OnClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(200))
                .Subscribe(_ => {
                    if (textManager.CurrentTextType == TextType.None)
                    {
                        var showText = homeModel.TextData.GetShowText(1, DB.TextTag.None);
                        textManager.SetTextType(showText);
                    }
                    else
                    {
                        textManager.OnClicked();
                    }
                })
                .AddTo(this);

            _babanukiButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => { baseModel.BasicData.Fullness -= 5; baseModel.BasicData.Money += 100; })
                .AddTo(this);

            _itemButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    _itemViewer.OpenViewer(homeModel.ItemData.ItemList);
                })
                .AddTo(this);

            _gearButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    //homeModel.ItemData.AddList();
                })
                .AddTo(this);

            _itemViewer.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(name =>
                {
                    //アイテム使用確認ダイアログ表示
                    _noticeDialogue.OpenDialogue("アイテムを使用します");
                    _noticeDialogue.OnButtonClicked
                        .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                        .Subscribe(_ =>
                        {
                            homeModel.ItemData.UseItem(name);
                            _noticeDialogue.CloseDialogue().Forget();
                            _itemViewer.UpdateDisp(homeModel.ItemData.ItemList);
                            _characterManager.ChangeFace(Face.Kirakira);
                        })
                        .AddTo(this)
                        .AddTo(_noticeDialogue.cancellationToken);
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

            //ステータスウィンドウの表示初期化
            //_statusWindowManager.Init(baseModel.BasicData.Affinity.ToString(), baseModel.BasicData.Weight.ToString(),
                //baseModel.BasicData.Attack.ToString(), baseModel.BasicData.Mental.ToString());
        }
    }
}