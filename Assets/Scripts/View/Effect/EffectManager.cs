using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TEMARI.View {
    /// <summary>
    /// �G�t�F�N�g���
    /// </summary>
    public enum EffectType
    {
        Hit = 0,
        Stun
    }

    /// <summary>
    /// �G�t�F�N�g�Ǘ��N���X
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
        /// �w�肵���G�t�F�N�g���Đ�
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
        /// �w�肵���G�t�F�N�g���~
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
