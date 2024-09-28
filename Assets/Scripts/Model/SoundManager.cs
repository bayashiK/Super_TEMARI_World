using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace TEMARI.Model
{
    /// <summary>
    /// サウンド管理クラス（シングルトン）
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        /// <summary> BGM種別 </summary>
        public enum BGMType
        {
            Title = 0,
            Home,
            Dungeon,
            None = 99
        }

        /// <summary> SE種別 </summary>
        public enum SEType
        {
            Click = 0,
            Cancel,
            PopUp,
            Enter,
            Damage,
            Critical,
            BodyAttack
        }

        /// <summary> シングルトンインスタンス </summary>
        public static SoundManager Instance;

        /// <summary> 基本データベース </summary>
        [SerializeField] private DB.BasicData _basicData;

        /// <summary> BGMオーディオソース </summary>
        private AudioSource[] _audioSourceBGM = new AudioSource[2];

        /// <summary> SEオーディオソース </summary>
        private AudioSource[] _audioSourceSE = new AudioSource[3];

        /// <summary> BGM </summary>
        [SerializeField] private AudioClip[] _audioClipBGM;

        /// <summary> SE </summary>
        [SerializeField] private AudioClip[] _audioClipSE;

        /// <summary> クロスフェード時間 </summary>
        private readonly float CrossFadeTime = 1.0f;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                _audioSourceBGM[0] = gameObject.AddComponent<AudioSource>();
                _audioSourceBGM[0].loop = true;
                _audioSourceBGM[1] = gameObject.AddComponent<AudioSource>();
                _audioSourceBGM[1].loop = true;

                for (int i = 0; i < _audioSourceSE.Length; i++)
                {
                    _audioSourceSE[i] = gameObject.AddComponent<AudioSource>();
                }
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetBGMVolume();
            SetSEVolume();
        }

        /// <summary>
        /// BGM音量セット
        /// </summary>
        /// <param name="value"></param>
        private void SetBGMVolume()
        {
            for (int i = 0; i < _audioSourceBGM.Length; i++)
            {
                _audioSourceBGM[i].volume = _basicData.BGMVolume;
            }
        }

        /// <summary>
        /// SE音量セット
        /// </summary>
        /// <param name="value"></param>
        private void SetSEVolume()
        {
            for (int i = 0; i < _audioSourceSE.Length; i++)
            {
                _audioSourceSE[i].volume = _basicData.SEVolume;
            }
        }

        /// <summary>
        /// BGM音量変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeBGMVolume(float volume)
        {
            _basicData.BGMVolume = volume;
            SetBGMVolume();
        }

        /// <summary>
        /// SE音量変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeSEVolume(float volume)
        {
            _basicData.SEVolume = volume;
            SetSEVolume();
        }

        /// <summary>
        /// BGM再生
        /// </summary>
        /// <param name="BGM"></param>
        /// <returns></returns>
        public async UniTaskVoid PlayBGM(BGMType BGM)
        {
            // BGMなしの状態にする場合            
            if (BGM == BGMType.None)
            {
                StopBGM();
                return;
            }

            int index = (int)BGM;
            
            if (index < 0 || _audioClipBGM.Length <= index)
            {
                return;
            }

            // 同じBGMの場合は何もしない
            if (_audioSourceBGM[0].clip != null && _audioSourceBGM[0].clip == _audioClipBGM[index])
            {
                return;
            }
            else if (_audioSourceBGM[1].clip != null && _audioSourceBGM[1].clip == _audioClipBGM[index])
            {
                return;
            }

            // フェードでBGM開始
            if (_audioSourceBGM[0].clip == null && _audioSourceBGM[1].clip == null)
            {
                _audioSourceBGM[0].clip = _audioClipBGM[index];
                _audioSourceBGM[0].Play();
            }
            else
            {
                // クロスフェード処理
                await CrossFadeChangeBMG(index);
            }
        }

        /// <summary>
        /// BGMのクロスフェード処理
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private async UniTask CrossFadeChangeBMG(int index)
        {
            int play = _audioSourceBGM[0].clip == null ? 0 : 1;
            int stop = 1 - play;

            // [play]が再生されている場合、[start]の音量を徐々に下げて、[stop]を新しい曲として再生
            _audioSourceBGM[play].volume = 0;
            _audioSourceBGM[play].clip = _audioClipBGM[index];
            _audioSourceBGM[play].Play();
            await DOTween.Sequence()
                .Append(_audioSourceBGM[play].DOFade(_basicData.BGMVolume, CrossFadeTime).SetEase(Ease.Linear))
                .Join(_audioSourceBGM[stop].DOFade(0, CrossFadeTime).SetEase(Ease.Linear));

            _audioSourceBGM[stop].Stop();
            _audioSourceBGM[stop].clip = null;
        }

        /// <summary>
        /// BGM完全停止
        /// </summary>
        public void StopBGM()
        {
            _audioSourceBGM[0].Stop();
            _audioSourceBGM[1].Stop();
            _audioSourceBGM[0].clip = null;
            _audioSourceBGM[1].clip = null;
        }

        /// <summary> SE再生 </summary>
        public void PlaySE(SEType SE)
        {
            int index = (int)SE;
            if (index < 0 || _audioClipSE.Length <= index)
            {
                return;
            }

            // 再生中ではないAudioSourceをつかってSEを鳴らす
            foreach (AudioSource source in _audioSourceSE)
            {
                // 再生中の AudioSource の場合には次のループ処理へ移る
                if (source.isPlaying)
                {
                    continue;
                }
                // 再生中でない AudioSource に Clip をセットして SE を鳴らす
                source.clip = _audioClipSE[index];
                source.Play();
                break;
            }
        }
    }
}