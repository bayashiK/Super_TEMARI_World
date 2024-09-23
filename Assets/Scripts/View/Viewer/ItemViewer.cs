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
    /// �A�C�e���r���[�A�Ǘ��N���X
    /// </summary>
    public class ItemViewer : MonoBehaviour
    {
        /// <summary> �A�C�e���f�[�^�x�[�X </summary>
        [SerializeField] protected DB.ItemData itemData;

        /// <summary> �A�C�e���v���n�u�̎Q�� </summary>
        [SerializeField] protected AssetReferenceGameObject itemPrefab;

        /// <summary> �r���[�A�{�̂�RectTransform </summary>
        protected RectTransform viewerRect;

        /// <summary> �A�C�e���̐e�I�u�W�F�N�g </summary>
        [SerializeField] protected Transform content;

        /// <summary> �r���[�A���o�� </summary>
        [SerializeField] protected TextMeshProUGUI caption;

        /// <summary> ����{�^�� </summary>
        [SerializeField] protected CustomButton closeButton;
        protected CloseButtonView closeButtonView;

        /// <summary> ���������v���n�u�̃��X�g </summary>
        protected Dictionary<string, ItemView> _prefabList = new();

        /// <summary> ���X�P�[�� </summary>
        protected float originalScale;

        /// <summary> �e�A�C�e���{�^���N���b�N�ʒm </summary>
        public IObservable<Unit> OnButtonClicked => _onButtonClicked;
        private Subject<Unit> _onButtonClicked = new();

        /// <summary> �I�𒆂̃A�C�e���� </summary>
        private string selectItemName;

        protected void Awake()
        {
            viewerRect = this.GetComponent<RectTransform>();
            closeButtonView = closeButton.GetComponent<CloseButtonView>();
        }

        protected void Start()
        {
            originalScale = viewerRect.localScale.x;

            //����{�^���N���b�N���_�C�A���O��\��
            closeButton.OnButtonClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => CloseViewer())
                .AddTo(this);

            _onButtonClicked.AddTo(this);
            this.gameObject.SetActive(false);
        }

        /// <summary>
        /// �r���[�A�\��
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
        /// �r���[�A��\��
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
        /// �A�C�e���v���n�u����
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
                    var buttonText = item.Type == DB.ItemType.Item ? "�g�p" : "����";
                    itemView.SetDisp(item.Name, item.Description, item.Possession, buttonText);
                    itemView.OnButtonClicked
                        .Subscribe(_ =>
                        {
                            selectItemName = item.Name; //�e�A�C�e���̃{�^���N���b�N���ɂ��̃A�C�e������ێ�
                            _onButtonClicked.OnNext(Unit.Default);
                        })
                        .AddTo(this)
                        .AddTo(itemView.gameObject);
                    _prefabList.Add(item.Name, itemView);
                }
            }
        }

        /// <summary>
        /// �\�����e�X�V
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
        /// �r���[�A���o���Z�b�g
        /// </summary>
        /// <param name="text"></param>
        public void SetCaption(string text)
        {
            caption.text = text;
        }
    }
}
