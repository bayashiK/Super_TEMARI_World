using System.Collections;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

namespace TEMARI.View
{
    /// <summary>
    /// 戦闘管理クラス
    /// </summary>
    public class BattleManager: MonoBehaviour
    {
        [SerializeField] private TemariSD _temari;
        [SerializeField] private SDBase _enemy;
        [SerializeField] private Image _positionMeter;
        [SerializeField] private Image _temariPos;
        [SerializeField] private Image _temariPos2;
        [SerializeField] private Image _enemyPos;
        [SerializeField] private Image _enemyPos2;
        private float _temariStartLine;
        private float _enemyStartLine;
        private float _ratio;
        private bool _start = false;

        private void Start()
        {
            var width = _positionMeter.rectTransform.sizeDelta.x - _temariPos.rectTransform.sizeDelta.x;
            _temariStartLine = _temari.GetsStartLine();
            _enemyStartLine = _enemy.GetsStartLine();
            _ratio = width / (Mathf.Abs(_temariStartLine) + Mathf.Abs(_enemyStartLine));

            _temari.Result
                .Subscribe(result =>
                {
                    _start = false;
                    _enemy.SetResult(!result);
                })
                .AddTo(this);

            _enemy.Result
                .Subscribe(result =>
                {
                    _start = false;
                    _temari.SetResult(!result);
                })
                .AddTo(this);
        }

        private void Update()
        {
            if (!_start)
            {
                return;
            }
            var currentTemariPos = _temari.Rect.localPosition;
            currentTemariPos.y = 0;
            var currentEnemyPos = _enemy.Rect.localPosition;
            currentEnemyPos.y = 0;
            _temariPos.rectTransform.localPosition = currentTemariPos * _ratio;
            _temariPos2.rectTransform.localPosition = currentTemariPos * _ratio;
            _enemyPos.rectTransform.localPosition = currentEnemyPos * _ratio;
            _enemyPos2.rectTransform.localPosition = currentEnemyPos * _ratio;
        }

        /// <summary>
        /// プレイヤーと敵移動開始
        /// </summary>
        public void StartRun()
        {
            _start = true;
            _temari.StartMove();
            _enemy.StartMove();
        }

        /// <summary>
        /// プレイヤー攻撃
        /// </summary>
        public void Attack()
        {
            _temari.Attack();
        }
    }
}