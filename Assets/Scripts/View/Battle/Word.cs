using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace TEMARI.View
{
    /// <summary>
    /// 攻撃ワードクラス
    /// </summary>
    public class Word: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;
        [SerializeField] private BoxCollider2D _collider;
        private const float MinWidth = 512f;
        private Vector3 _speed;
        private float _scale;
        private RectTransform _rect;
        public int Attack { private set; get; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="collider"></param>
        /// <param name="text"></param>
        /// <param name="dest"></param>
        /// <param name="speed"></param>
        public void Init(BoxCollider2D collider, string text, float dest, float speed, int attack)
        {
            Physics2D.IgnoreCollision(_collider, collider, true); //生成元との衝突判定を無効化
            _text.text = text;
            Attack = attack;
            var width = Mathf.Clamp(_text.preferredWidth + 150f, MinWidth, 1000);
            var height = _text.preferredHeight + 80f;
            var size = new Vector2(width, height);
            _image.rectTransform.sizeDelta = size;
            _collider.size = size;
            _speed = new Vector3(speed, 0, 0);
            _rect = this.transform as RectTransform;
            _scale = _rect.localScale.x;
            var dist = Mathf.Abs(dest - _rect.localPosition.x);
            //Debug.Log($"destination:{dest}, posx:{_rect.localPosition.x}, distance:{dist}, speed:{speed}");
            _rect.DOLocalMoveX(dest, dist / speed).SetEase(Ease.Linear);
        }

        /// <summary>
        /// 破壊時の処理
        /// </summary>
        private async void Burst()
        {
            _rect.DOKill();
            _collider.enabled = false;
            await _rect.DOScale(1.3f * _scale, 0.05f).SetEase(Ease.InBack);
            Destroy(this.gameObject);
        }

        protected void OnCollisionEnter2D(Collision2D collision)
        {
            Burst();
        }
    }
}