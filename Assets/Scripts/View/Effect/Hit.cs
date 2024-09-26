using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace TEMARI.View
{
    /// <summary>
    /// 攻撃ヒットエフェクトクラス
    /// </summary>
    public class Hit : EffectBase
    {
        [SerializeField] private Image _star1;
        [SerializeField] private Image _star2;
        [SerializeField] private float _radius = 150f;
        [SerializeField] private bool _isPlayer = false;
        private RectTransform _star1Rect;
        private RectTransform _star2Rect;

        private void Start()
        {
            _star1Rect = _star1.rectTransform;
            _star2Rect = _star2.rectTransform;
            SetActive(false);
        }

        /// <summary>
        /// エフェクト再生
        /// </summary>
        public override async void Play()
        {
            var rad1 = Random.Range(60, 80) * Mathf.Deg2Rad;
            var rad2 = Random.Range(20, 40) * Mathf.Deg2Rad;
            var x1 = Mathf.Cos(rad1) * _radius;
            var y1 = Mathf.Sin(rad1) * _radius;
            var x2 = Mathf.Cos(rad2) * _radius;
            var y2 = Mathf.Sin(rad2) * _radius;
            if (_isPlayer)
            {
                x1 *= -1;
                x2 *= -1;
            }

            SetActive(true);
            var tasks = new UniTask[] {
                _star1Rect.DOLocalMove(new Vector2(x1, y1), 0.2f).ToUniTask(),
                _star1Rect.DOLocalRotate(new Vector3(0, 0, 360), 0.2f).ToUniTask(),
                _star2Rect.DOLocalMove(new Vector2(x2, y2), 0.2f).ToUniTask(),
                _star2Rect.DOLocalRotate(new Vector3(0, 0, 360), 0.2f).ToUniTask()
            };
            await tasks;
            SetActive(false);
            _star1Rect.localPosition = Vector2.zero;
            _star2Rect.localPosition= Vector2.zero;
        }

        /// <summary>
        /// エフェクト停止
        /// </summary>
        public override void Stop()
        {

        }

        private void SetActive(bool active)
        {
            _star1.gameObject.SetActive(active);
            _star2.gameObject.SetActive(active);
        }
    }
}
