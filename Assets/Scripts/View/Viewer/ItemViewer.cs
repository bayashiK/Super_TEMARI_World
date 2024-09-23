using DG.Tweening;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Linq;
using TEMARI.Model;

namespace TEMARI.View
{
    /// <summary>
    /// アイテムビューア管理クラス
    /// </summary>
    public class ItemViewer : MonoBehaviour
    {
        /// <summary> アイテムデータベース </summary>
        [SerializeField] protected DB.ItemData itemData;

        /// <summary> アイテムプレハブの参照 </summary>
        [SerializeField] protected AssetReferenceGameObject itemPrefab;

        /// <summary> ビューア本体のRectTransform </summary>
        protected RectTransform viewerRect;

        /// <summary> アイテムの親オブジェクト </summary>
        [SerializeField] protected Transform content;

        /// <summary> ビューア見出し </summary>
        [SerializeField] protected TextMeshProUGUI caption;

        /// <summary> 閉じるボタン </summary>
        [SerializeField] protected CustomButton closeButton;
        protected CloseButtonView closeButtonView;

        /// <summary> 生成したプレハブのリスト </summary>
        protected Dictionary<string, ItemView> _prefabList = new();

        /// <summary> 元スケール </summary>
        protected float originalScale;

        /// <summary> 各アイテムボタンクリック通知 </summary>
        public IObservable<Unit> OnButtonClicked => _onButtonClicked;
        private Subject<Unit> _onButtonClicked = new();

        /// <summary> 選択中のアイテム名 </summary>
        private string selectItemName;

        protected void Awake()
        {
            viewerRect = this.GetComponent<RectTransform>();
            closeButtonView = closeButton.GetComponent<CloseButtonView>();
        }

        protected void Start()
        {
            originalScale = viewerRect.localScale.x;

            //閉じるボタンクリック時ダイアログ非表示
            closeButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => CloseViewer())
                .AddTo(this);

            _onButtonClicked.AddTo(this);
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// ビューア表示
        /// </summary>
        /// <param name="active"></param>
        public async void OpenViewer()
        {
            Model.SoundManager.Instance.PlaySE(Model.SoundManager.SEType.PopUp);
            this.gameObject.SetActive(true);

            _ = InstantiateItemPrefab(itemData.ItemList);

            closeButtonView.SetDefaultColor();
            viewerRect.localScale = Vector2.zero;
            await viewerRect.DOScale(originalScale, 0.15f).SetEase(Ease.OutBack, 1);
        }

        /// <summary>
        /// ビューア非表示
        /// </summary>
        public async void CloseViewer()
        {
            Model.SoundManager.Instance.PlaySE(Model.SoundManager.SEType.Cancel);
            await viewerRect.DOScale(0, 0.15f).SetEase(Ease.Linear);
            this.gameObject.SetActive(false);

            foreach(var item in _prefabList)
            {
                Destroy(item.Value.gameObject);
            }
            _prefabList.Clear();
        }

        /// <summary>
        /// アイテムプレハブ生成
        /// </summary>
        /// <param name="items"></param>
        private async UniTask InstantiateItemPrefab(IReadOnlyCollection<DB.Item> items)
        {
            foreach (var item in items)
            {
                if (item.Possession > 0)
                {
                    var obj = await itemPrefab.InstantiateAsync(content);
                    var itemView = obj.GetComponent<ItemView>();
                    var buttonText = item.Type == DB.ItemType.Item ? "使用" : "装備";
                    itemView.SetDisp(item.Name, item.Description, item.Possession, buttonText);
                    itemView.OnButtonClicked
                        .Subscribe(_ =>
                        {
                            selectItemName = item.Name; //各アイテムのボタンクリック時にそのアイテム名を保持
                            _onButtonClicked.OnNext(Unit.Default);
                        })
                        .AddTo(this)
                        .AddTo(itemView.gameObject);
                    _prefabList.Add(item.Name, itemView);
                }
            }
        }

        /// <summary>
        /// 表示内容更新
        /// </summary>
        /// <param name="items"></param>
        public void UpdateDisp()
        {
            itemData.UseItem(selectItemName);
            foreach (var item in itemData.ItemList)
            {
                if (_prefabList.ContainsKey(item.Name))
                {
                    var prefab = _prefabList[item.Name];
                    if (item.Possession > 0)
                    {
                        prefab.SetItemPossession(item.Possession);
                    }
                    else
                    {
                        _prefabList.Remove(item.Name);
                        Destroy(prefab.gameObject);
                    }
                }                
            }
        }

        /// <summary>
        /// ビューア見出しセット
        /// </summary>
        /// <param name="text"></param>
        public void SetCaption(string text)
        {
            caption.text = text;
        }
    }
}
