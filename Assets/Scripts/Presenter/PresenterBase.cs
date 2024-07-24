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
            textManager.SetTextSpeed(baseModel.BasicData.GetTextSpeedValue());
            Model.SoundManager.Instance.SetBGMVolume(baseModel.BasicData.BGMVolume);
            Model.SoundManager.Instance.SetSEVolume(baseModel.BasicData.SEVolume);
            settingDialogue.SetSliderValue(baseModel.BasicData.TextSpeedInt, baseModel.BasicData.BGMVolume, baseModel.BasicData.SEVolume);

            //設定ダイアログ表示
            settingButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => settingDialogue.SetDialogueActive(true))
                .AddTo(this);

            //テキスト表示スピード変更
            settingDialogue.OnTextSpeedChanged
                .Subscribe(x => {
                    baseModel.BasicData.TextSpeedInt = (int)x;
                    textManager.SetTextSpeed(baseModel.BasicData.GetTextSpeedValue());
                })
                .AddTo(this);

            //BGM音量変更
            settingDialogue.OnBGMVolumeChanged
                .Subscribe(x => {
                    baseModel.BasicData.BGMVolume = x;
                    Model.SoundManager.Instance.SetBGMVolume(baseModel.BasicData.BGMVolume);
                })
                .AddTo(this);

            //SE音量変更
            settingDialogue.OnSEVolumeChanged
                .Subscribe(x => {
                    baseModel.BasicData.SEVolume = x;
                    Model.SoundManager.Instance.SetSEVolume(baseModel.BasicData.SEVolume);
                })
                .AddTo(this);

            //設定ダイアログ閉じる
            settingDialogue.CloseDialogue
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => settingDialogue.SetDialogueActive(false))
                .AddTo(this);

            //会話テキストボックスの表示状態切り替え
            baseModel.IsTextEnabled
                .Subscribe(x => textManager.SetTextBoxActive(x, baseModel.AllText))
                .AddTo(this);

            //会話テキストを最後まで表示完了
            textManager.TextFinish
                .Subscribe(_ =>
                {
                    baseModel.SetTextType(View.TextType.None);
                })
                .AddTo(this);

            //画面クリックのテキスト飛ばし
            bgMouseManager.OnClicked
                .Where(_ => baseModel.IsTextEnabled.Value != View.TextType.None)
                .ThrottleFirst(TimeSpan.FromMilliseconds(200))
                .Subscribe(_ => textManager.OnClicked())
                .AddTo(this);
        }
    }
}