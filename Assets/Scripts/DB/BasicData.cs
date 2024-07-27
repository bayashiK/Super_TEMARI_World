using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace TEMARI.DB
{
    [CreateAssetMenu(fileName = "BasicData", menuName = "ScriptableObject/BasicData")]
    public class BasicData : ScriptableObject
    {
        /// <summary> マニー最大所持数 </summary>
        public static readonly int MaxMoney = 999999;
        /// <summary> 親愛度最大値 </summary>
        public static readonly int MaxAffinity = 100;
        /// <summary> 満腹度最大値 </summary>
        public static readonly int MaxFullness = 50;
        /// <summary> 体重最小値 </summary>
        public static readonly int MinWeight = 51;
        /// <summary> 体重最大値 </summary>
        public static readonly int MaxWeight = 99;
        /// <summary> 口撃力最大値 </summary>
        public static readonly int MaxAttack = 100;
        /// <summary> メンタル最大値 </summary>
        public static readonly int MaxMental = 100;

        [Header("マニー所持数")]
        [SerializeField] private int _money = 0;
        /// <summary> マニー所持数 </summary>
        public int Money {
            get { return _money; }
            set {
                _money = Mathf.Clamp(value + _money, 0, MaxMoney);
                _onMoneyChanged.OnNext(_money); 
            }
        }

        /// <summary> マニー所持数変化通知 </summary>
        public IObservable<int> OnMoneyChanged => _onMoneyChanged;
        protected Subject<int> _onMoneyChanged = new();

        [Header("親愛度")]
        [SerializeField] private int _affinity = 0;
        /// <summary> 親愛度 </summary>
        public int Affinity
        {
            get { return _affinity; }
            set { _affinity = Mathf.Clamp(value + _affinity, 0, MaxAffinity); }
        }

        [Header("満腹度")]
        [SerializeField] private int _fullness = MaxFullness; 
        /// <summary> 満腹度 </summary>
        public int Fullness
        {
            get { return _fullness; }
            set { 
                _fullness = Mathf.Clamp(value + _fullness, 0, MaxFullness);
                _onFullnessChanged.OnNext(_fullness);
            }
        }

        /// <summary> 満腹度変化通知 </summary>
        public IObservable<int> OnFullnessChanged => _onFullnessChanged;
        protected Subject<int> _onFullnessChanged = new();

        [Header("体重")]
        [SerializeField] private int _weight = 1;
        /// <summary> 体重 </summary>
        public int Face
        {
            get { return _weight; }
            set { _weight = Mathf.Clamp(value + _weight, MinWeight, MaxWeight); }
        }

        [Header("口撃力")]
        [SerializeField] private int _attack = 1;
        /// <summary> 口撃力 </summary>
        public int Attack
        {
            get { return _attack; }
            set { _attack = Mathf.Clamp(value + _attack, 1, MaxAttack); }
        }

        [Header("メンタル")]
        [SerializeField] private int _mental = 0;
        /// <summary> メンタル </summary>
        public int Mental
        {
            get { return _mental; }
            set { _mental = Mathf.Clamp(value + _mental, 0, MaxMental); }
        }

        /// <summary> テキスト表示スピード定義 </summary>
        public static ReadOnlyCollection<float> TextSpeed = Array.AsReadOnly(new float[] { 0.15f, 0.1f, 0.05f });

        /// <summary> 現在のテキスト表示スピード（定義のインデックス） </summary>
        [SerializeField] private int _textSpeedInt = 1;
        public int TextSpeedInt
        {
            get { return _textSpeedInt; }
            set { _textSpeedInt = value; }
        }
        /// <summary> 現在のテキスト表示スピードを実際の値で返す </summary>
        /// <returns></returns>
        public float GetTextSpeedValue()
        {
            return TextSpeed[_textSpeedInt];
        }

        /// <summary> 現在のBGM音量 </summary>
        public float BGMVolume
        {
            get { return _BGMVolume; }
            set { _BGMVolume = Mathf.Clamp(value, 0, 1); }
        }
        [SerializeField] private float _BGMVolume = 0.5f;

        /// <summary> 現在のSE音量 </summary>
        public float SEVolume
        {
            get { return _SEVolume; }
            set { _SEVolume = Mathf.Clamp(value, 0, 1); }
        }
        [SerializeField] private float _SEVolume = 0.5f;

        public Dictionary<string, List<bool>> sceneFlags = new Dictionary<string, List<bool>>()
        {
            {"Home", new List<bool>(){false, false, false}}
        };

        public void SaveChange()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        /*
        [System.Serializable]
        public class SceneFlag 
        {
            public string sceneName;
            public Model.SceneBase.GameState gameState;
        }
        */
    }
}