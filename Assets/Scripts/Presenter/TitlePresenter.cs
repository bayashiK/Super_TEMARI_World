using Cysharp.Threading.Tasks;
using TEMARI.Model;
using System;
using UniRx;
using UnityEngine;

namespace TEMARI.Presenter
{
    /// <summary>
    /// �^�C�g����ʃv���[���^�[
    /// </summary>
    public class TitlePresenter : PresenterBase
    {
        protected override void Start()
        {
            SoundManager.Instance.PlayBGM(SoundManager.BGMType.Title).Forget();
            base.Start();
        }

        protected override void Init()
        {
            base.Init();

            //��ʃN���b�N�ŃX�^�[�g
            bgMouseManager.OnClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(200))
                .Subscribe(async _ => {
                    SoundManager.Instance.PlaySE(SoundManager.SEType.Enter);
                    await baseModel.ChangeSceneAsync("Home");
                    })
                .AddTo(this);
        }
    }
}
