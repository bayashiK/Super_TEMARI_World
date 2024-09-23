using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TEMARI.DB;

namespace TEMARI.Model
{
    /// <summary>
    /// 画面ベースクラス
    /// </summary>
    public class SceneBase : MonoBehaviour
    {
        /// <summary> 基本データベース </summary>
        [SerializeField] protected DB.BasicData basicData;
        /// <summary> 基本データベース </summary>
        public DB.BasicData BasicData {  get { return basicData; } }

        /// <summary> destroyCancellationToken </summary>
        protected CancellationToken dct; 

        /// <summary> シーン遷移時アニメーション用キャンバス </summary>
        [SerializeField] protected GameObject sceneCanvas;
        protected Image scenePanel;
        protected TextMeshProUGUI sceneText;
        protected readonly string loadingText = "Now Loading";

        /// <summary> エラー通知イベント </summary>
        public IObservable<string> OnError => onError;
        protected Subject<string> onError = new();

        /// <summary> 現在のシーン名 </summary>
        protected string sceneName;

        protected virtual void Start()
        {
            dct = destroyCancellationToken;
            scenePanel = sceneCanvas.transform.Find("Panel").GetComponent<Image>();
            sceneText = sceneCanvas.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            sceneCanvas.SetActive(false);
            onError.AddTo(this);
            sceneName = SceneManager.GetActiveScene().name;
            try
            {
                var type = (SoundManager.BGMType)Enum.Parse(typeof(SoundManager.BGMType), sceneName, true);
                SoundManager.Instance.PlayBGM(type).Forget(); //現在のシーン名に対応するBGMを再生する
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                SoundManager.Instance.PlayBGM(SoundManager.BGMType.None).Forget(); //対応するBGMがない場合は再生を停止する
            }
        }

        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public async UniTask ChangeSceneAsync(string sceneName)
        {
            sceneCanvas.SetActive(true);
            DisplayLoadingTextAsync(dct).Forget();
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            var tasks = new List<UniTask>();
            tasks.Add(asyncLoad.ToUniTask());
            tasks.Add(scenePanel.DOFade(1, 0.3f).ToUniTask());
            //basicData.SaveChange();
            asyncLoad.allowSceneActivation = true;
            await tasks;
        }

        /// <summary>
        /// NowLoading表示アニメーション
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async UniTask DisplayLoadingTextAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                sceneText.text = loadingText;
                await UniTask.Delay(TimeSpan.FromMilliseconds(150), cancellationToken: ct);
                sceneText.text = loadingText + "・";
                await UniTask.Delay(TimeSpan.FromMilliseconds(150), cancellationToken: ct);
                sceneText.text = loadingText + "・・";
                await UniTask.Delay(TimeSpan.FromMilliseconds(150), cancellationToken: ct);
                sceneText.text = loadingText + "・・・";
                await UniTask.Delay(TimeSpan.FromMilliseconds(150), cancellationToken: ct);
            }
        }
    }
}