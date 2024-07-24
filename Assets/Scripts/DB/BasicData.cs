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
        /// <summary> コイン最大所持数 </summary>
        private static int MaxCoin = 999999;
        /// <summary> 幸運最大値 </summary>
        public static int MaxLuck = 20;
        /// <summary> 表情筋最大値 </summary>
        public static int MaxFace = 20;
        /// <summary> 洞察力最大値 </summary>
        public static int MaxInsight = 20;
        /// <summary> 交渉力最大値 </summary>
        public static int MaxNegotiation = 10;

        [Header("コイン所持数")]
        [SerializeField] private int _coin = 0;
        /// <summary> コイン所持数 </summary>
        public int Coin {
            get { return _coin; }
            set {
                var addCoin = value + CalcCoinCoefficient(value);
                _coin = Mathf.Clamp(addCoin + _coin, 0, MaxCoin);
                _onCoinChanged.OnNext(_coin); 
            }
        }

        /// <summary> コイン所持数変化通知 </summary>
        public IObservable<int> OnCoinChanged => _onCoinChanged;
        protected Subject<int> _onCoinChanged = new();

        [Header("幸運値")]
        [SerializeField] private int _luck = 1;
        /// <summary> 幸運値 </summary>
        public int Luck
        {
            get { return _luck; }
            set { _luck = Mathf.Clamp(value + _luck, 1, MaxLuck); }
        }

        [Header("表情筋")]
        [SerializeField] private int _face = 1;
        /// <summary> 表情筋 </summary>
        public int Face
        {
            get { return _face; }
            set { _face = Mathf.Clamp(value + _face, 1, MaxFace); }
        }

        [Header("洞察力")]
        [SerializeField] private int _insight = 1;
        /// <summary> 洞察力 </summary>
        public int Insight
        {
            get { return _insight; }
            set { _insight = Mathf.Clamp(value + _insight, 1, MaxInsight); }
        }

        [Header("交渉力")]
        [SerializeField] private int _negotiation = 0;
        /// <summary> 交渉力 </summary>
        public int Negotiotion
        {
            get { return _negotiation; }
            set { _negotiation = Mathf.Clamp(value + _negotiation, 0, MaxNegotiation); }
        }

        /// <summary>
        /// コイン獲得倍率計算
        /// </summary>
        /// <returns></returns>
        public int CalcCoinCoefficient(int value)
        {
            return value > 0 ? (int)Mathf.Ceil(value * _negotiation * 0.1f) : 0;
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