using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

namespace TEMARI.View
{
    /// <summary>
    /// SD�퓬���[�V�����N���X
    /// </summary>
    public class SDBase : MonoBehaviour
    {
        /// <summary> ����X�v���C�g </summary>
        [SerializeField] protected Image runSprite;

        /// <summary> �_���[�W���󂯂�X�v���C�g </summary>
        [SerializeField] protected Image damageSprite;

        /// <summary> �_���[�W���󂯂��ۂ̃p�[�e�B�N�� </summary>
        [SerializeField] protected ParticleSystem damageParticle;

        /// <summary> ���t�U���v���n�u�̎Q�� </summary>
        [SerializeField] protected AssetReferenceGameObject wordPrefab;

        /// <summary> �ړ����x </summary>
        [SerializeField] protected float moveSpeed = 50;

        /// <summary> ���t�U���̑��x </summary>
        [SerializeField] protected float wordSpeed = 70;

        /// <summary> �R���C�_�[ </summary>
        private BoxCollider2D _collider;

        /// <summary> �_���[�W�ʒm�C�x���g </summary>
        public IObservable<int> OnDamaged => onDamaged;
        protected Subject<int> onDamaged = new();

        /// <summary> �f�t�H���g��y���W </summary>
        protected float defaultY;

        /// <summary> �ړ���̍��W </summary>
        protected float destination = 1000;

        /// <summary> �̓����莞�̃m�b�N�o�b�N </summary>
        protected float knockBackBody = -200;

        /// <summary> ���t�U�����󂯂��ۂ̃m�b�N�o�b�N </summary>
        protected float knockBackWord = -100;

        /// <summary> �m�b�N�o�b�N�t���O </summary>
        protected bool isKnockBacked = false;

        /// <summary> ��b </summary>
        protected List<string> wordList = new();

        protected RectTransform _rect;

        private CancellationTokenSource cts = new CancellationTokenSource();

        protected virtual void Start()
        {
            damageSprite.gameObject.SetActive(false);
            _collider = this.GetComponent<BoxCollider2D>();
            _rect = this.transform as RectTransform;
            defaultY = _rect.localPosition.y;
        }
        
        /// <summary>
        /// �ړ��J�n
        /// </summary>
        public void StartMove()
        {
            runSprite.gameObject.SetActive(true);
            damageSprite.gameObject.SetActive(false);
            var dist = Mathf.Abs(destination - _rect.localPosition.x);
            _rect.DOLocalJump(new Vector2(destination, defaultY), 30, (int)dist / 40, dist / moveSpeed);
        }

        /// <summary>
        /// ���t�𔭎˂��čU��
        /// </summary>
        public virtual async void Attack()
        {
            if (isKnockBacked)
            {
                return;
            }

            await KillMoveTween(0.2f);
            _ = _rect.DOLocalJump(_rect.localPosition, 40, 1, 0.4f);
            /*
            float defY = _rect.localPosition.y;
            _ = DOTween.Sequence().Append(_rect.DOLocalMoveY(defY + 40, 0.2f))
                .Append(_rect.DOLocalMoveY(defY, 0.2f).SetEase(Ease.InQuad));
            */

            cts = new();
            
            try
            {
                await UniTask.WhenAll(InstantiateWord(), UniTask.Delay(TimeSpan.FromMilliseconds(500), cancellationToken: cts.Token));
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString());
                return;
            }
            StartMove();
        }

        /// <summary>
        /// ���t�U���v���n�u�̐���
        /// </summary>
        /// <returns></returns>
        protected async UniTask InstantiateWord()
        {
            var obj = await wordPrefab.InstantiateAsync(this.transform.parent);
            obj.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
            var objRect = obj.transform as RectTransform;
            objRect.localPosition = _rect.localPosition + new Vector3(-knockBackBody, UnityEngine.Random.Range(-20, 20) + defaultY, 0);
            var word = obj.GetComponent<Word>();
            var text = wordList[UnityEngine.Random.Range(0, wordList.Count)];
            word.Init(_collider, text, destination, wordSpeed);
        }

        /// <summary>
        /// �m�b�N�o�b�N���̃g�D�C�[��
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual async UniTask KnockBack(GameObject obj, CancellationToken token)
        {
            isKnockBacked = true;
            var currentX = _rect.localPosition.x;
            damageParticle.Play();
            try
            {
                if (obj.CompareTag("Word"))
                {
                    obj.GetComponent<Word>().Burst();
                    _ = this.transform.DOLocalMoveX(currentX + knockBackWord, 0.3f);
                    await UniTask.Delay(TimeSpan.FromMilliseconds(300), cancellationToken: token);
                }
                else
                {
                    _ = _rect.DOLocalMoveX(currentX + knockBackWord, 0.6f);
                    await UniTask.Delay(TimeSpan.FromMilliseconds(600), cancellationToken: token);
                }
            }
            catch(OperationCanceledException e)
            {
                Debug.Log(e.ToString());
                return;
            }
            isKnockBacked = false;
            StartMove();
        }

        /// <summary>
        /// �ړ��g�D�C�[���̒�~
        /// </summary>
        protected async UniTask KillMoveTween(float time)
        {
            _rect.DOKill();
            await _rect.DOLocalMoveY(defaultY, time);
        }

        /// <summary>
        /// �ڐG��
        /// </summary>
        /// <param name="collision"></param>
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            _ =  KillMoveTween(0);
            if (isKnockBacked)
            {
                cts.Cancel();
            }
            else
            {
                runSprite.gameObject.SetActive(false);
                damageSprite.gameObject.SetActive(true);
            }
            
            cts = new();
            _ = KnockBack(collision.gameObject, cts.Token);
        }
    }
}