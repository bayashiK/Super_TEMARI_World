using System;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TEMARI.View
{
    /// <summary>
    /// 背景マウスイベントクラス
    /// </summary>
    [RequireComponent(typeof(ObservableEventTrigger))]
    public class BGMouseManager : MonoBehaviour
    {
        private Image _backGround;
        private ObservableEventTrigger _observableEvenTrigger;

        /// <summary>
        /// クリック時
        /// </summary>
        public IObservable<Unit> OnClicked => _observableEvenTrigger
            .OnPointerClickAsObservable().AsUnitObservable();

        /// <summary>
        /// 押下時
        /// </summary>
        public IObservable<Unit> OnPressed => _observableEvenTrigger
            .OnPointerDownAsObservable().AsUnitObservable();

        /// <summary>
        /// 解放時
        /// </summary>
        public IObservable<Unit> OnReleased => _observableEvenTrigger
            .OnPointerUpAsObservable().AsUnitObservable();

        public IObservable<PointerEventData> OnDrag => _observableEvenTrigger
            .OnDragAsObservable();

        private void Awake()
        {
            _backGround = this.GetComponent<Image>();
            _observableEvenTrigger = GetComponent<ObservableEventTrigger>();
        }
    }
}