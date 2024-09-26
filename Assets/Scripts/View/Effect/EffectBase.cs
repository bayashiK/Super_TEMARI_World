using UnityEngine;

namespace TEMARI.View
{
    /// <summary>
    /// エフェクト基底クラス
    /// </summary>
    public abstract class EffectBase : MonoBehaviour
    {
        /// <summary>
        /// エフェクト再生
        /// </summary>
        public abstract void Play();

        /// <summary>
        /// エフェクト停止
        /// </summary>
        public abstract void Stop();
    }
}
