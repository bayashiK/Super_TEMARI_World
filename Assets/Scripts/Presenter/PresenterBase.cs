using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.Presenter
{
    /// <summary>
    /// プレゼンターベースクラス
    /// </summary>
    public class PresenterBase : MonoBehaviour
    {
        /// <summary> 画面モデル </summary>
        [SerializeField] protected Model.SceneBase baseModel;

        /// <summary> 背景マウスイベントマネージャー </summary>
        [SerializeField] protected View.BGMouseManager bgMouseManager;

        /// <summary> テキストマネージャー </summary>
        [SerializeField] protected View.TextManager textManager;

        /// <summary> 設定ダイアログマネージャー </summary>
        [SerializeField] protected View.SettingDialogue settingDialogue;
        /// <summary> 設定画面ボタン </summary>
        [SerializeField] protected View.CustomButton settingButton;

        /// <summary> エラーダイアログ </summary>
        [SerializeField] protected View.NoticeDialogue errorDialogue;

        protected virtual void Start()
        {
            /*
            //エラー発生時通知ダイアログ表示
            baseModel.OnError
                .Subscribe(x =>
                {
                    errorDialogue.SetDialogueActive(true);
                    errorDialogue.SetMainText(x);
                })
                .AddTo(this);

            errorDialogue.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(async _ =>
                {
                    errorDialogue.SetDialogueActive(false);
                    await baseModel.ChangeSceneAsync("Title");
                })
                .AddTo(this);

            errorDialogue.CloseDialogue
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(async _ =>
                {
                    errorDialogue.SetDialogueActive(false);
                    await baseModel.ChangeSceneAsync("Title");
                })
                .AddTo(this);

            await UniTask.WaitUntil(() => baseModel.isLoaded);
            if (baseModel.isLoadSucceeded)
            {
                Init();
            }
            */
            Init();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected virtual void Init()
        {
            //設定ダイアログ表示
            settingButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => settingDialogue.OpenDialogue().Forget())
                .AddTo(this);

            //テキスト表示スピード変更
            settingDialogue.OnTextSpeedChanged
                .Subscribe(x => {
                    textManager.ChangeTextSpeed((int)x);
                })
                .AddTo(this);

            //BGM音量変更
            settingDialogue.OnBGMVolumeChanged
                .Subscribe(x => {
                    Model.SoundManager.Instance.ChangeBGMVolume(x);
                })
                .AddTo(this);

            //SE音量変更
            settingDialogue.OnSEVolumeChanged
                .Subscribe(x => {
                    Model.SoundManager.Instance.ChangeSEVolume(x);
                })
                .AddTo(this);

            //画面クリックのテキスト飛ばし
            bgMouseManager.OnClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(200))
                .Subscribe(_ => textManager.OnClicked())
                .AddTo(this);
        }
    }
}