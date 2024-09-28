using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using System;
using DG.Tweening;

namespace TEMARI.View
{
    /// <summary>
    /// 報酬画面表示クラス
    /// </summary>
    public class ResultDisplay: MonoBehaviour
    {
        /// <summary> WIN表示 </summary>+
        [SerializeField] private Image _win;
        private RectTransform _winRect;
        private float _winScale;

        /// <summary> LOSE表示 </summary>
        [SerializeField] private Image _lose;
        private RectTransform _loseRect;
        private float _loseScale;

        /// <summary> 報酬ダイアログ </summary>
        [SerializeField] private RewordDialogue _dialogue;

        /// <summary> グレーアウト用背景 </summary>
        [SerializeField] private BGMouseManager _background;

        private bool _isFnish = false;

        void Start()
        {
            _winRect = _win.GetComponent<RectTransform>();
            _loseRect = _lose.GetComponent<RectTransform>();
            _winScale = _winRect.localScale.x;
            _loseScale = _loseRect.localScale.x;
            Hidden();

            _dialogue.OnButtonClicked
                .Subscribe(_ =>
                {
                    Hidden();
                })
                .AddTo(this);

            _background.OnClicked
                .Subscribe(_ =>
                {
                    if (_isFnish)
                    {
                        Hidden();
                    }
                })
                .AddTo(this);
        }

        /// <summary>
        /// リザルト表示
        /// </summary>
        /// <param name="win"></param>
        /// <returns></returns>
        public async UniTask DisplayResult(bool win)
        {
            _isFnish = false;
            this.gameObject.SetActive(true);
            if (win)
            {
                await TweenWin();
                _dialogue.OpenDialogue("1007 マニー");
            }
            else
            {
                await TweenLose();
            }
        }

        private async UniTask TweenWin()
        {
            await _winRect.DOScale(0, 0);
            _win.gameObject.SetActive(true);
            await _winRect.DOScale(_winScale, 0.2f).SetEase(Ease.OutBack);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            await _winRect.DOLocalMoveY(250, 0.2f);
        }

        private async UniTask TweenLose()
        {
            await _lose.DOFade(0, 0);
            _lose.gameObject.SetActive(true);
            await _lose.DOFade(1, 0.5f);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            _isFnish = true;
        }

        private void Hidden()
        {
            _win.gameObject.SetActive(false);
            _lose.gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}