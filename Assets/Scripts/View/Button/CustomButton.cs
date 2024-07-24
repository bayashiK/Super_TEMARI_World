using TEMARI.Model;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TEMARI.View
{
    /// <summary>
    /// �ėp�{�^��
    /// </summary>
    [RequireComponent(typeof(ObservableEventTrigger))]
    public class CustomButton : MonoBehaviour
    {
        private ObservableEventTrigger _observableEvenTrigger;

        /// <summary>
        /// �{�^���N���b�N��
        /// </summary>
        public IObservable<Unit> OnButtonClicked => _observableEvenTrigger
            .OnPointerClickAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// �{�^��������
        /// </summary>
        public IObservable<Unit> OnButtonPressed => _observableEvenTrigger
            .OnPointerDownAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// �{�^�������
        /// </summary>
        public IObservable<Unit> OnButtonReleased => _observableEvenTrigger
            .OnPointerUpAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// �{�^���̗̈�ɃJ�[�\�����������Ƃ�
        /// </summary>
        public IObservable<Unit> OnButtonEntered => _observableEvenTrigger
            .OnPointerEnterAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// �{�^���̗̈悩��J�[�\�����o���Ƃ�
        /// </summary>
        public IObservable<Unit> OnButtonExited => _observableEvenTrigger
            .OnPointerExitAsObservable().AsUnitObservable().Where(_ => _isActiveRP.Value);

        /// <summary>
        /// �{�^���̃A�N�e�B�u��ԕێ�
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
        /// �{�^���̃A�N�e�B�u��Ԏ擾
        /// </summary>
        public bool GetIsActive() => _isActiveRP.Value;

        /// <summary>
        /// �{�^���̃A�N�e�B�u��ԕύX
        /// </summary>
        /// <returns></returns>
        public void SetActive(bool isActive)
        {
            _isActiveRP.Value = isActive;
        }
    }
}
