using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace TEMARI.View
{
    /// <summary>
    /// メーター管理クラス
    /// </summary>
    public class MeterManager: MonoBehaviour
    {
        /// <summary> メーター </summary>
        [SerializeField] private Image _meterValue;

        /// <summary> メーター減少時の値差分 </summary>
        [SerializeField] private Image _valueDiff;

        /// <summary >メーターの初期値 </summary>
        private float _defaultValue;

        /// <summary >現在のメーターの値(1～0の割合) </summary>
        private float _currentRatio = 1;

        private CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>メーターの残量に応じた色指定</summary>
        //private Color32[] _meterColor = { new Color32(150, 255, 100, 255), new Color32(255, 240, 90, 255), new Color32(240, 80, 80, 255) };

        void Start()
        {
            _meterValue.DOFillAmount(1, 0);
            _valueDiff?.DOFillAmount(1, 0);
        }

        public void Init(float value)
        {
            _defaultValue = value;
            _currentRatio = 1;
        }

        /// <summary>
        /// メーターの表示を更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async UniTask UpdateValue(float value)
        {
            var ratio = value / _defaultValue;
            var ratioDiff = _currentRatio - ratio;
            _ = _valueDiff?.DOFillAmount(ratio, ratioDiff * 2).SetEase(Ease.Linear);
            await _meterValue.DOFillAmount(ratio, ratioDiff * 2).SetEase(Ease.Linear);
        }

        /// <summary>
        /// メーターの表示を更新
        /// </summary>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public async UniTask UpdateValue(float value, float time)
        {
            var ratio = value / _defaultValue;
            _ = _valueDiff?.DOFillAmount(ratio, time).SetEase(Ease.Linear);
            await _meterValue.DOFillAmount(ratio, time).SetEase(Ease.Linear);
        }

        /// <summary>
        /// メーターの値を減少(差分を遅延表示)
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public async UniTask DecreaseValueDelay(float value)
        {
            /*
            _valueDiff.color = ratio switch
            {
                > 0.5f => _meterColor[0],
                > 0.2f => _meterColor[1],
                _ => _meterColor[2]
            };
            */
            cts.Cancel();
            cts = new();
            var ratio = value / _defaultValue;
            var ratioDiff = _currentRatio - ratio;
            try
            {
                _ = _meterValue.DOFillAmount(ratio, ratioDiff / 10);
                await UniTask.Delay(TimeSpan.FromSeconds(ratioDiff / 10 + 0.5f), cancellationToken: cts.Token);
                _ = _valueDiff.DOFillAmount(ratio, ratioDiff * 2);
                await UniTask.Delay(TimeSpan.FromSeconds(ratioDiff * 2), cancellationToken: cts.Token);
                _currentRatio = ratio;
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString());
                return;
            }
        }
    }
}