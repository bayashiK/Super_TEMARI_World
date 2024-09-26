using UnityEngine;

namespace TEMARI.View
{
    /// <summary>
    /// �G�t�F�N�g���N���X
    /// </summary>
    public abstract class EffectBase : MonoBehaviour
    {
        /// <summary>
        /// �G�t�F�N�g�Đ�
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// �G�t�F�N�g��~
        /// </summary>
        public abstract void Stop();
    }
}
