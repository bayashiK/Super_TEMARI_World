using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;
using System;
using UniRx;
using UnityEngine.EventSystems;

namespace TEMARI.View
{
    /// <summary>
    /// トランプカードビュークラス
    /// </summary>
    [RequireComponent(typeof(ObservableEventTrigger))]
    public class TrumpCard : MonoBehaviour
    {
        /// <summary> 手札内でのインデックス情報 </summary>
        public int HandIndex { get; set; }

        private ObservableEventTrigger _observableEvenTrigger;

        /// <summary> イベント受付可否 </summary>
        public bool IsTriggerEnabled { get; set; } = false;
        /// <summary> 横軸移動可否 </summary>
        public bool Horizontal { get; set; } = true;
        /// <summary> 縦軸移動可否 </summary>
        public bool Vertical { get; set; } = true;
        /// <summary> ドラッグ開始時の座標 </summary>
        private Vector2 _onBeginPos;
        /// <summary> ドラッグ開始時のヒエラルキー順序 </summary>
        private int _onBeginIndex= 0;
        /// <summary> ドラッグ移動閾値 </summary>
        private float _dragMoveThreshold = 100f;

        /// <summary> ドラッグ移動完了通知(正規化した方向ベクトル) </summary>
        public IObservable<Vector2> OnDragged => _onDragged;
        private Subject<Vector2> _onDragged = new();

        /// <summary> クリック時 </summary>
        public IObservable<Unit> OnClicked => _observableEvenTrigger
            .OnPointerClickAsObservable().AsUnitObservable();

        /// <summary> カード </summary>
        [SerializeField] private Image _card;

        /// <summary> RectTransform </summary>
        private RectTransform _rect;
        /// <summary> AnchoredPosition </summary>
        public Vector2 AnchoredPos { get { return _rect.anchoredPosition; } }

        /// <summary> 裏面のスプライト </summary>
        private Sprite _backSprite;
        /// <summary> 表面のスプライト </summary>
        private Sprite _faceSprite;
        /// <summary> 表面かどうか </summary>
        private bool isFace = false;

        /// <summary> カードをめくるスピード </summary>
        private float turnSpeed = 0.1f;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _observableEvenTrigger = GetComponent<ObservableEventTrigger>();

            //ドラッグ開始時の座標記録
            _observableEvenTrigger
                .OnPointerDownAsObservable()
                .Where(_ => IsTriggerEnabled)
                .Subscribe(_ =>
                {
                    _onBeginPos = _rect.position;
                    _onBeginIndex = transform.GetSiblingIndex();
                    transform.SetAsLastSibling();
                })
                .AddTo(this);

            //ドラッグ移動
            _observableEvenTrigger
                .OnBeginDragAsObservable()
                .SelectMany(_observableEvenTrigger.OnDragAsObservable())
                .TakeUntil(_observableEvenTrigger.OnEndDragAsObservable())
                .Where(_ => IsTriggerEnabled)
                .Select(x => GetWorldPointInRect(x))
                .Pairwise() // NOTE: Buffer(2,1)だと最後にペアでない値が来る
                .RepeatUntilDestroy(this)
                .Subscribe(DragMove)
                .AddTo(this);

            //ドラッグ終了時に移動距離が閾値以下ならドラッグ開始時の位置に戻す
            //一定以上移動していたならthisを通知する
            _observableEvenTrigger
                .OnPointerUpAsObservable()
                .Where(_ => IsTriggerEnabled)
                .Subscribe(_ => { 
                    Vector2 completePos = _rect.position; 
                    if(Vector2.Distance(_onBeginPos, completePos) < _dragMoveThreshold)
                    {
                        UndoDrag();
                    }
                    else
                    {
                        var vec = (completePos - _onBeginPos).normalized;
                        _onDragged.OnNext(vec);
                    }
                })
                .AddTo(this);

            /*
            OnClicked
                .ThrottleFirst(TimeSpan.FromMilliseconds(500))
                .Subscribe(async _ => await TurnCard())
                .AddTo(this);
            */

            _backSprite = _card.sprite;
        }

        /// <summary>
        /// 画面座標をRectTransform上のワールド座標に変換
        /// </summary>
        /// <param name="eventData"></param>
        /// <returns></returns>
        private Vector3 GetWorldPointInRect(PointerEventData eventData)
        {
            var screenPosition = eventData.position;
            var result = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_rect, screenPosition, eventData.pressEventCamera, out result);
            return result;
        }

        /// <summary>
        /// RectTransform上のワールド座標ペアから相対的な移動先を計算し移動
        /// </summary>
        /// <param name="positions"></param>
        private void DragMove(Pair<Vector3> positions)
        {
            var deltaX = Horizontal ? positions.Current.x - positions.Previous.x : 0;
            var deltaY = Vertical ? positions.Current.y - positions.Previous.y : 0;
            //Debug.Log($"rect:{_rect.position}, prev:{positions.Previous}, current:{positions.Current}");
            _rect.position += new Vector3(deltaX, deltaY, 0);
        }

        /// <summary>
        /// カードを裏返す
        /// </summary>
        /// <returns></returns>
        public async UniTask TurnCard()
        {
            if (_faceSprite == null) return;
            await _rect.DOScaleX(0, turnSpeed);
            isFace = isFace ? false : true;
            _card.sprite = isFace ? _faceSprite : _backSprite;
            await _rect.DOScaleX(1, turnSpeed);
        }

        /// <summary>
        /// 移動するTween
        /// </summary>
        /// <param name="dist"></param>
        /// <param name="duraration"></param>
        /// <returns></returns>
        public DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> 
            Move(Vector2 dist, float duraration)
        {
            return _rect.DOAnchorPos(dist, duraration);
        }

        /// <summary>
        /// 表面のスプライトセット
        /// </summary>
        /// <param name="sprite"></param>
        public void SetFaceSprite(Sprite sprite)
        {
            _faceSprite = sprite;
        }

        /// <summary>
        /// ドラッグ前の座標に戻す
        /// </summary>
        public void UndoDrag()
        {
            _rect.position = _onBeginPos;
            transform.SetSiblingIndex(_onBeginIndex);
        }

        public void SetWorldPosZero()
        {
            _rect.position = Vector3.zero;
        }
    }
}