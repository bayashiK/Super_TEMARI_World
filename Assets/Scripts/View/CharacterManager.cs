using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace TEMARI.View
{
    /// <summary>
    /// キャラクター表示クラス
    /// </summary>

    public class CharacterManager : MonoBehaviour
    {
        /// <summary> 素体画像 </summary>
        [SerializeField] private Image _base;

        /// <summary> 服画像 </summary>
        [SerializeField] private Image _clothes;

        /// <summary> 表情画像 </summary>
        [SerializeField] private Image _face;

        /// <summary> 手画像 </summary>
        [SerializeField] private Image _hand;

        private ObservableEventTrigger _observableEvenTrigger;

        private List<AsyncOperationHandle<Sprite>> _handleList = new();

        private Sprite _nomal;
        private Sprite _fear;
        private Sprite _jitome;
        private Sprite _kirakira;
        private Sprite _sad;
        private Sprite _smile;
        private Sprite _tere;

        /// <summary> クリック時 </summary>
        public IObservable<Unit> OnClicked => _observableEvenTrigger
            .OnPointerClickAsObservable().AsUnitObservable();

        void Awake()
        {
            _observableEvenTrigger = _base.GetComponent<ObservableEventTrigger>();
        }

        async void Start()
        {
            _handleList.Add(Addressables.LoadAssetAsync<Sprite>("Character/face_normal.png"));
            _handleList.Add(Addressables.LoadAssetAsync<Sprite>("Character/face_fear.png"));
            _handleList.Add(Addressables.LoadAssetAsync<Sprite>("Character/face_jitome.png"));
            _handleList.Add(Addressables.LoadAssetAsync<Sprite>("Character/face_kirakira.png"));
            _handleList.Add(Addressables.LoadAssetAsync<Sprite>("Character/face_sad.png"));
            _handleList.Add(Addressables.LoadAssetAsync<Sprite>("Character/face_smile.png"));
            _handleList.Add(Addressables.LoadAssetAsync<Sprite>("Character/face_tere.png"));
            var tasks = new List<UniTask>();
            foreach (var handle in _handleList)
            {
                tasks.Add(handle.ToUniTask());
            }

            await tasks;
            _nomal = _handleList[0].Result;
            _fear = _handleList[1].Result;
            _jitome = _handleList[2].Result;
            _kirakira = _handleList[3].Result;
            _sad = _handleList[4].Result;
            _smile = _handleList[5].Result;
            _tere = _handleList[6].Result;
        }

        /// <summary>
        /// 表情画像を変更する
        /// </summary>
        /// <param name="face"></param>
        public void ChangeFace(Face face)
        {
            _face.sprite = face switch
            {
                Face.Fear => _fear,
                Face.Kirakira => _kirakira,
                Face.Jitome => _jitome,
                Face.Sad => _sad,
                Face.Smile => _smile,
                Face.Tere => _tere,
                _ => _nomal
            };
        }

        private void OnDestroy()
        {
            foreach (var handle in _handleList)
            {
                Addressables.Release(handle);
            }
        }
    }

    /// <summary>
    /// 表情の種類
    /// </summary>
    public enum Face
    {
        Normal,
        Fear,
        Jitome,
        Kirakira,
        Sad,
        Smile,
        Tere
    }
}