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
        public DB.BasicData BasicData { get { return basicData; } }

        /*
        /// <summary> データベースロード終了 </summary>
        public bool isLoaded = false;
        /// <summary> データベースロード成否 </summary>
        public bool isLoadSucceeded = false;
        */

        /// <summary> 表示テキスト全文 </summary>
        public IReadOnlyCollection<string> AllText => allText;
        protected List<string> allText;

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
            /*
            await LoadMasterData();
            if (!isLoadSucceeded)
            {
                onError.OnNext("データの読み込みで\nエラーが発生したため\nタイトルに戻ります");
                return;
            }
            */
            LoadText();
            dct = destroyCancellationToken;
            scenePanel = sceneCanvas.transform.Find("Panel").GetComponent<Image>();
            sceneText = sceneCanvas.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            sceneCanvas.SetActive(false);
            onError.AddTo(this);
            sceneName = SceneManager.GetActiveScene().name;
        }

        /*
        /// <summary>
        /// マスタ読み込み
        /// </summary>
        protected virtual async UniTask LoadMasterData()
        {
            LoadText();
            var handle = Addressables.LoadAssetAsync<DB.BasicData>("MasterData/BasicData");
            await handle.Task;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                basicData = handle.Result;
                isLoadSucceeded =  true;
            }
            else
            {
                Debug.Log("failed");
                basicData = null;
                isLoadSucceeded =  false;
            }
            isLoaded = true;
        }
        */

        /*
        /// <summary>
        /// 破棄時にマスタ解放
        /// </summary>
        protected void OnDestroy()
        {
            if (basicData != null)
            {
                //Debug.Log("master released");
                Addressables.Release(basicData);
            }
        }
        */

        /// <summary>
        /// 会話テキストをセット
        /// </summary>
        public void LoadText()
        {
            allText = new List<string>() { "テストメッセージ\nテスト1テスト1テスト1", "テストメッセージ\nテスト2テスト2テスト2", "ラストメッセージ\nラストメッセージ" };
        }

        /// <summary>
        /// 画面遷移
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public async UniTask ChangeSceneAsync(string sceneName)
        {
            /*
            sceneName = "Scenes/" + sceneName;
            sceneCanvas.SetActive(true);
            var handle =  Addressables.LoadSceneAsync(sceneName);
            DisplayLoadingTextAsync(dct).Forget();
            await UniTask.WhenAll(scenePanel.DOFade(1, 0.3f).ToUniTask(), handle.Task.AsUniTask());
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var scene = handle.Result.Scene;
                SceneManager.SetActiveScene(scene);
            }
            else
            {
                sceneCanvas.SetActive(false);
                onError.OnNext("シーンの読み込みで\nエラーが発生しました");
            }
            */
            
            sceneCanvas.SetActive(true);
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;
            DisplayLoadingTextAsync(dct).Forget();
            //basicData.SaveChange();
            await scenePanel.DOFade(1, 0.3f);
            asyncLoad.allowSceneActivation = true;
            await asyncLoad;
            
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