using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;

namespace TEMARI.View
{
    /// <summary>
    /// キャラクター表示クラス
    /// </summary>
    [RequireComponent(typeof(ObservableEventTrigger))]
    public class CharacterManager : MonoBehaviour
    {
        private ObservableEventTrigger _observableEvenTrigger;

        /// <summary> クリック時 </summary>
        public IObservable<Unit> OnClicked => _observableEvenTrigger
            .OnPointerClickAsObservable().AsUnitObservable();

        /// <summary> キャラクター画像 </summary>
        [SerializeField] private Image _image;

        void Awake()
        {
            _observableEvenTrigger = GetComponent<ObservableEventTrigger>();
        }

        void Start()
        {

        }
    }
}