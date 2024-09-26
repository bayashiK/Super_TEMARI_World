using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TEMARI.View {
    /// <summary>
    /// エフェクト種別
    /// </summary>
    public enum EffectType
    {
        Hit = 0,
        Stun
    }

    /// <summary>
    /// エフェクト管理クラス
    /// </summary>
    public class EffectManager : MonoBehaviour
    {
        [SerializeField] private EffectBase[] _effects = { null, null };

        void Start()
        {
            foreach (EffectType value in Enum.GetValues(typeof(EffectType)))
            {
                var transform = this.transform.Find(value.ToString());
                _effects[(int)value] = transform?.GetComponent<EffectBase>();
            }
        }

        /// <summary>
        /// 指定したエフェクトを再生
        /// </summary>
        /// <param name="type"></param>
        public void Play(EffectType type)
        {
            if(_effects[(int)type] != null)
            {
                _effects[(int)type].Play();
            }
        }

        /// <summary>
        /// 指定したエフェクトを停止
        /// </summary>
        /// <param name="type"></param>
        public void Stop(EffectType type)
        {
            if (_effects[(int)type] != null)
            {
                _effects[(int)type].Stop();
            }
        }
    }
}
