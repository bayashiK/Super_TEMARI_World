using TEMARI.Model;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TEMARI.View
{
    /// <summary>
    /// 汎用ボタン
    /// </summary>
    [RequireComponent(typeof(ObservableEventTrigger))]
    public class CustomButton : MonoBehaviour
    {
        private ObservableEventTrigger _observableEvenTrigger;

        /// <summary>
        /// ボタンクリック時
        /// </summary>
        public IObservable<Unit> OnButtonClicked => _observableEvenTrigger
            .OnPointerClickAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// ボタン押下時
        /// </summary>
        public IObservable<Unit> OnButtonPressed => _observableEvenTrigger
            .OnPointerDownAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// ボタン解放時
        /// </summary>
        public IObservable<Unit> OnButtonReleased => _observableEvenTrigger
            .OnPointerUpAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// ボタンの領域にカーソルが入ったとき
        /// </summary>
        public IObservable<Unit> OnButtonEntered => _observableEvenTrigger
            .OnPointerEnterAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// ボタンの領域からカーソルが出たとき
        /// </summary>
        public IObservable<Unit> OnButtonExited => _observableEvenTrigger
            .OnPointerExitAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// ボタンのアクティブ状態保持
        /// </summary>
        public IReadOnlyReactiveProperty<bool> IsActiveRP => _isActiveRP;

        private readonly BoolReactiveProperty _isActiveRP = new BoolReactiveProperty(true);

        protected virtual void OnDestroy()
        {
            _isActiveRP.Dispose();
        }
        
        protected virtual void Awake()
        {
            _observableEvenTrigger = GetComponent<ObservableEventTrigger>();
        }

        /// <summary>
        /// ボタンのアクティブ状態取得
        /// </summary>
        public bool GetIsActive() => _isActiveRP.Value;

        /// <summary>
        /// ボタンのアクティブ状態変更
        /// </summary>
        /// <returns></returns>
        public void SetActive(bool isActive)
        {
            _isActiveRP.Value = isActive;
        }
    }
}
