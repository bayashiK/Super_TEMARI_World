using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace TEMARI.View
{
    /// <summary>
    /// 気絶エフェクトクラス
    /// </summary>
    public class Stun : EffectBase
    {
        [SerializeField] private Image _ray;
        [SerializeField] private Image _star1;
        [SerializeField] private Image _star2;
        [SerializeField] private float MinScale = 0.2f;
        [SerializeField] private float MaxScale = 0.45f;
        [SerializeField] private float _aroundTime = 0.5f;
        private RectTransform _star1Rect;
        private RectTransform _star2Rect;
        private Vector3 _frontPos = new Vector3(0, 110, 0);
        private Vector3 _backPos = new Vector3(0, 150, 0);
        private Vector3 _rightPos = new Vector3(50, 130, 0);
        private Vector3 _leftPos = new Vector3(-50, 130, 0);

        private void Start()
        {
            _star1Rect = _star1.rectTransform;
            _star2Rect = _star2.rectTransform;
            Init();
            SetActive(false);
        }

        /// <summary>
        /// エフェクト再生
        /// </summary>
        public override void Play()
        {
            SetActive(true);
            _star1Rect.DOLocalPath(new[] {_backPos, _leftPos, _frontPos, _rightPos}, _aroundTime, PathType.CatmullRom)
                .SetOptions(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            _star1Rect.DOScale(MaxScale, _aroundTime / 2).SetLoops(-1, LoopType.Yoyo);
            _star2Rect.DOLocalPath(new[] { _frontPos, _rightPos, _backPos, _leftPos, }, _aroundTime, PathType.CatmullRom)
                .SetOptions(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            _star2Rect.DOScale(MinScale, _aroundTime / 2).SetLoops(-1, LoopType.Yoyo);
        }

        /// <summary>
        /// エフェクト停止
        /// </summary>
        public override void Stop()
        {
            _star1Rect.DOKill();
            _star2Rect.DOKill();
            SetActive(false);
            Init();
        }

        private void SetActive(bool active)
        {
            _ray.gameObject.SetActive(active);
            _star1.gameObject.SetActive(active);
            _star2.gameObject.SetActive(active);
        }

        private void Init()
        {
            _star1Rect.localPosition = _backPos;
            _star1Rect.localScale = new Vector2(MinScale, MinScale);
            _star2Rect.localPosition = _frontPos;
            _star2Rect.localScale = new Vector2(MaxScale, MaxScale);
        }
    }
}
