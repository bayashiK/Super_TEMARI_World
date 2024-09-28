using System.Collections;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;

namespace TEMARI.View
{
    /// <summary>
    /// 戦闘管理クラス
    /// </summary>
    public class BattleManager: MonoBehaviour
    {
        /// <summary> プレイヤーSD </summary>
        [SerializeField] private TemariSD _player;

        /// <summary> 敵SD </summary>
        [SerializeField] private SDBase _enemy;

        /// <summary> プレイヤーHPゲージ </summary>
        [SerializeField] protected MeterManager _playerHPMeter;

        /// <summary> 敵HPゲージ </summary>
        [SerializeField] protected MeterManager _enemyHPMeter;

        /// <summary> 残弾アイコンのリスト </summary>
        [SerializeField] private List<RemainingWordIcon> _wordIcons;
        private int _remainingIndex;
        private Transform _iconsParent;

        /// <summary> リロード中のゲージ </summary>
        [SerializeField] private MeterManager _reloadGage;

        /// <summary> プレイヤーの状態表示テキスト </summary>
        [SerializeField] private TextMeshProUGUI _playerStateText;

        /// <summary> 敵の状態表示テキスト </summary>
        [SerializeField] private TextMeshProUGUI _enemyStateText;

        /// <summary> 位置表示メーター </summary>
        [SerializeField] private Image _positionMeter;
        [SerializeField] private Image _playerPos;
        [SerializeField] private Image _playerPos2;
        [SerializeField] private Image _enemyPos;
        [SerializeField] private Image _enemyPos2;

        /// <summary> リザルト表示 </summary>
        [SerializeField] private ResultDisplay _resultDisp;

        private float _playerStartLine;
        private float _enemyStartLine;
        private float _ratio;
        private bool _start = false;

        private void Start()
        {
            _playerHPMeter.Init(_player.GetHp());
            _reloadGage.Init(1);
            int i = 1;
            foreach (var wordIcon in _wordIcons) {
                if(i > _player.AttackCapacity)
                {
                    wordIcon.gameObject.SetActive(false);
                }
                i++;
            }
            _iconsParent = _wordIcons[0].transform.parent;

            _player.RemainingWordNum
                .Subscribe(async num =>
                {
                    _wordIcons[_remainingIndex].ChangeState(false);
                    _remainingIndex++;
                    if(num <= 0)
                    {
                        _reloadGage.gameObject.SetActive(true);
                        await _reloadGage.UpdateValue(1, _player.ReloadTime);
                        await _reloadGage.UpdateValue(0, 0);
                        _reloadGage.gameObject.SetActive(false);
                        for (int i = 0; i < _player.AttackCapacity; i++)
                        {
                            _wordIcons[i].ChangeState(true);
                        }
                        _remainingIndex = 0;
                        await _iconsParent.DOScale(1.2f, 0.1f);
                        _ =  _iconsParent.DOScale(1, 0.1f).SetEase(Ease.InQuad);
                    }
                })
                .AddTo(this);

            _player.OnDamaged
                .Subscribe(hp =>
                {
                    _ = _playerHPMeter.DecreaseValueDelay(hp);
                })
                .AddTo(this);

            _player.OnDown
                .Subscribe(time =>
                {
                    _ = DisplayDownText(true, time);
                })
                .AddTo(this);

            ResetState();
        }

        public void ResetState()
        {
             _ = _playerHPMeter.UpdateValue(_player.GetHp(), 0);
            _enemyHPMeter.Init(_enemy.GetHp());
            _ = _reloadGage.UpdateValue(0, 0);
            _reloadGage.gameObject.SetActive(false);
            _remainingIndex = 0;
            for (int i = 0; i < _player.AttackCapacity; i++)
            {
                _wordIcons[i].ChangeState(true);
            }
            _playerStateText.gameObject.SetActive(false);
            _enemyStateText.gameObject.SetActive(false);

            var width = _positionMeter.rectTransform.sizeDelta.x - _playerPos.rectTransform.sizeDelta.x;
            _playerStartLine = _player.GetsStartLine();
            _enemyStartLine = _enemy.GetsStartLine();
            _ratio = width / (Mathf.Abs(_playerStartLine) + Mathf.Abs(_enemyStartLine));

            _player.ResetPosition();
            _enemy.ResetPosition();

            _enemy.OnDamaged
               .Subscribe(hp =>
               {
                   _ = _enemyHPMeter.DecreaseValueDelay(hp);
               })
               .AddTo(this);

            _enemy.OnDown
                .Subscribe(time =>
                {
                    _ = DisplayDownText(false, time);
                })
                .AddTo(this);

            _player.Result
                .Subscribe(async result =>
                {
                    _start = false;
                    _enemy.SetResult(!result);
                    await _resultDisp.DisplayResult(result);
                })
                .AddTo(this)
                .AddTo(_enemy);

            _enemy.Result
                .Subscribe(async result =>
                {
                    _start = false;
                    _player.SetResult(!result);
                    await _resultDisp.DisplayResult(!result);
                })
                .AddTo(this)
                .AddTo(_enemy);

            _start = true;
        }

        private void Update()
        {
            if (!_start)
            {
                return;
            }

            // SDキャラの位置に連動して、位置メーター上のアイコンの位置も更新
            var currentTemariPos = _player.Rect.localPosition;
            currentTemariPos.y = 0;
            var currentEnemyPos = _enemy.Rect.localPosition;
            currentEnemyPos.y = 0;
            _playerPos.rectTransform.localPosition = currentTemariPos * _ratio;
            _playerPos2.rectTransform.localPosition = currentTemariPos * _ratio;
            _enemyPos.rectTransform.localPosition = currentEnemyPos * _ratio;
            _enemyPos2.rectTransform.localPosition = currentEnemyPos * _ratio;
        }

        /// <summary>
        /// プレイヤーと敵移動開始
        /// </summary>
        public void StartRun()
        {
            _start = true;
            _player.StartMove();
            _enemy.StartMove();
        }

        /// <summary>
        /// プレイヤー攻撃
        /// </summary>
        public void Attack()
        {
            _player.Attack();
        }

        /// <summary>
        /// ダウン状態のテキスト表示
        /// </summary>
        /// <param name="isPlayer"></param>
        /// <param name="downTime"></param>
        /// <returns></returns>
        private async UniTask DisplayDownText(bool isPlayer, float downTime)
        {
            var targetText = isPlayer ? _playerStateText : _enemyStateText;
            targetText.text = "Down !";
            targetText.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromSeconds(downTime - 2));
            _ = targetText.DOFade(0, 0.25f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            targetText.DOKill();
            await targetText.DOFade(1, 0);
            _ = targetText.DOFade(0, 0.125f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            targetText.DOKill();
            await targetText.DOFade(1, 0);
            targetText.gameObject.SetActive(false);
        }
    }
}