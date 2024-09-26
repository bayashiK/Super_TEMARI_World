using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

namespace TEMARI.View
{
    public enum SpriteType
    {
        Run = 0,
        Damage,
        Down,
        Special
    }

    /// <summary>
    /// SD�퓬���[�V�����N���X
    /// </summary>
    public class SDBase : MonoBehaviour
    {
        [Header("�X�v���C�g�̃��X�g�@(���聨�_���[�W���_�E��������̏��œo�^����)")]
        /// <summary> �X�v���C�g�̃��X�g </summary>
        [SerializeField] protected List<Image> sprites;

        /// <summary> ��e����C�⎞�̃G�t�F�N�g�Ǘ� </summary>
        [SerializeField] protected EffectManager effectManager;

        /// <summary> HP�Q�[�W�Ǘ� </summary>
        [SerializeField] protected MeterManager HPMeter;

        /// <summary> ���t�U���v���n�u�̎Q�� </summary>
        [SerializeField] protected AssetReferenceGameObject wordPrefab;

        /// <summary> �̗� </summary>
        [SerializeField] protected int hp = 100;

        /// <summary> �U���� </summary>
        [SerializeField] protected int attack = 20;

        /// <summary> �h��� </summary>
        [SerializeField] protected int defence = 10;

        /// <summary> �ړ����x </summary>
        [SerializeField] protected float moveSpeed = 50;

        /// <summary> ���t�U���̑��x </summary>
        [SerializeField] protected float wordSpeed = 70;

        /// <summary> �̓����莞�̃m�b�N�o�b�N </summary>
        [SerializeField] protected float knockBackBody = -200;

        /// <summary> ���t�U�����󂯂��ۂ̃m�b�N�o�b�N </summary>
        [SerializeField] protected float knockBackWord = -100;

        /// <summary> �_�E����������(�b) </summary>
        [SerializeField] protected float downTime = 3;

        /// <summary> �R���C�_�[ </summary>
        private BoxCollider2D _collider;

        /// <summary> �f�t�H���g��y���W </summary>
        protected float defaultY;

        /// <summary> �ړ���̍��W </summary>
        protected float destination = 620;

        /// <summary> �������W </summary>
        protected float startLine = -830;

        /// <summary> �m�b�N�o�b�N�t���O </summary>
        protected bool isKnockBack = false;

        /// <summary> �_�E����ԃt���O </summary>
        protected bool isDown = false;

        /// <summary> ���G��ԃt���O </summary>
        protected bool isInvincible = false;

        /// <summary> ���s�ʒm�C�x���g </summary>
        public IObservable<bool> Result => result;
        protected Subject<bool> result = new();

        /// <summary> ��b </summary>
        protected List<string> wordList = new();

        public RectTransform Rect { private set; get; }

        protected CancellationTokenSource cts = new CancellationTokenSource();
        protected CancellationTokenSource ctsDown = new CancellationTokenSource();

        protected void Awake()
        {
            _collider = this.GetComponent<BoxCollider2D>();
            Rect = this.transform as RectTransform;
            startLine = Rect.localPosition.x;
            defaultY = Rect.localPosition.y;
        }

        protected virtual void Start()
        {
            SetActive(SpriteType.Run);
            HPMeter.Init(hp);
        }
        
        /// <summary>
        /// �ړ��J�n
        /// </summary>
        public void StartMove()
        {
            SetActive(SpriteType.Run);
            var dist = Mathf.Abs(destination - Rect.localPosition.x);
            Rect.DOLocalJump(new Vector2(destination, defaultY), 30, (int)dist / 40, dist / moveSpeed).SetEase(Ease.Linear);
        }

        /// <summary>
        /// ���t�𔭎˂��čU��
        /// </summary>
        public virtual async void Attack()
        {
            if (isKnockBack || isDown)
            {
                return;
            }

            await KillMoveTween(0.2f);
            _ = Rect.DOLocalJump(Rect.localPosition, 40, 1, 0.4f);
            /*
            float defY = Rect.localPosition.y;
            _ = DOTween.Sequence().Append(Rect.DOLocalMoveY(defY + 40, 0.2f))
                .Append(Rect.DOLocalMoveY(defY, 0.2f).SetEase(Ease.InQuad));
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
        /// ���s�̃Z�b�g
        /// </summary>
        /// <param name="win"></param>
        public virtual void SetResult(bool win)
        {
            cts.Cancel();
            _ = KillMoveTween(0);
            if (win)
            {
                SetActive(SpriteType.Run);
            }
            else
            {
                SetActive(SpriteType.Down);
                result.OnNext(false);
            }
        }

        public float GetsStartLine()
        {
            return startLine;
        }

        /// <summary>
        /// �w�肵���X�v���C�g�ȊO���\���ɂ���
        /// </summary>
        /// <param name="active"></param>
        protected void SetActive(SpriteType type)
        {
            int index = (int)type;
            if(sprites.Count <= index)
            {
                return;
            }
            for (int i = 0; i < sprites.Count; i++)
            {
                if (i == index)
                {
                    sprites[i].gameObject.SetActive(true);
                }
                else
                {
                    sprites[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// ���t�U���v���n�u�̐���
        /// </summary>
        /// <returns></returns>
        protected async UniTask InstantiateWord()
        {
            var obj = await wordPrefab.InstantiateAsync(this.transform.parent);
            //obj.transform.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
            var objRect = obj.transform as RectTransform;
            objRect.localPosition = Rect.localPosition + new Vector3(-knockBackBody, UnityEngine.Random.Range(-20, 20) + defaultY, 0);
            var word = obj.GetComponent<Word>();
            var text = wordList[UnityEngine.Random.Range(0, wordList.Count)];
            word.Init(_collider, text, -startLine, wordSpeed, attack);
        }

        /// <summary>
        /// �m�b�N�o�b�N�̃g�D�C�[��
        /// </summary>
        /// <param name="backDistannce"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual async UniTask KnockBack(float backDistannce, CancellationToken token)
        {
            try
            {
                if (isDown)
                {
                    _ = Rect.DOShakePosition(0.3f, 20, 10, 1);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: token);
                }
                else
                {
                    isKnockBack = true;
                    SetActive(SpriteType.Damage);
                    float distance = 0;
                    if (backDistannce > 0)
                    {
                        distance = Mathf.Clamp(Rect.localPosition.x + backDistannce, destination, startLine);
                    }
                    else
                    {
                        distance = Mathf.Clamp(Rect.localPosition.x + backDistannce, startLine, destination);
                    }
                    float time = Mathf.Abs(backDistannce) * 3 / 1000;

                    if (distance == startLine)  //�m�b�N�o�b�N�ɂ���ăX�^�[�g���C���܂Ŗ߂��ꂽ�ꍇ
                    {
                        await this.transform.DOLocalMoveX(distance, time);
                        isKnockBack = false;
                        _ = Down();
                        return;
                    }
                    
                    _ = this.transform.DOLocalMoveX(distance, time);
                    await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: token);
                    isKnockBack = false;
                    StartMove();
                }
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString());
                return;
            }
        }

        /// <summary>
        /// �_�E�����̏���
        /// </summary>
        /// <returns></returns>
        protected async UniTask Down()
        {
            isDown = true;
            ctsDown = new();
            SetActive(SpriteType.Down);
            effectManager.Play(EffectType.Stun);
            var defaultDef = defence;
            defence = 0;
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(downTime), cancellationToken: ctsDown.Token);
                defence = defaultDef;
                StartMove();
                _ = ToInvincible();
            }
            catch (OperationCanceledException e)
            {
                Debug.Log(e.ToString());
            }
            effectManager.Stop(EffectType.Stun);
            isDown = false;
        }

        /// <summary>
        /// ��莞�Ԗ��G��
        /// </summary>
        /// <returns></returns>
        protected async UniTask ToInvincible()
        {
            isInvincible = true;
            var defColor = sprites[0].color;
            var fadeColor = new Color32(255, 255, 255, 50);
            for (int i = 0; i < 15; i++)
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(100));
                sprites[0].color = fadeColor;
                await UniTask.Delay(TimeSpan.FromMilliseconds(100));
                sprites[0].color = defColor;
            }
            isInvincible = false;
        }

        /// <summary>
        /// �ړ��g�D�C�[���̒�~
        /// </summary>
        protected async UniTask KillMoveTween(float time)
        {
            Rect.DOKill();
            await Rect.DOLocalMoveY(defaultY, time);
        }

        /// <summary>
        /// �ڐG��
        /// </summary>
        /// <param name="collision"></param>
        protected async void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isInvincible)
            {
                _ = KillMoveTween(0);
            }
            if (isKnockBack || isDown)
            {
                cts.Cancel();
            }
            
            cts = new();
            var obj = collision.gameObject;

            if (obj.CompareTag("Word")) //���t�U����e
            {
                var word = obj.GetComponent<Word>();
                word.Burst();
                if (!isInvincible)
                {
                    int damage = Mathf.Clamp(word.Attack - defence, 1, hp);
                    hp -= damage;
                    _ = HPMeter.DecreaseValueDelay(hp);
                    effectManager.Play(EffectType.Hit);

                    if (hp <= 0)
                    {
                        cts.Cancel();
                        ctsDown.Cancel();
                        SetResult(false);
                    }
                    else
                    {
                        await KnockBack(knockBackWord, cts.Token);
                    }
                }
            }
            else if (!isDown) //�̓������e
            {
                if (!isInvincible)
                {
                    await KnockBack(knockBackBody, cts.Token);
                }
            }
        }
    }
}