using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity1Week
{
    // 普通のプラットフォーム。乗るとスコア加算
    public class PlatformNormal : PlatformBehaviour, ICustomDragEvent
    {
        [SerializeField]
        private int score = 1;

        [SerializeField]
        private Color landedColor = Color.white;

        [SerializeField]
        private SpriteRenderer spriteRenderer;

        [SerializeField]
        private Collider2D anyCollider;

        [SerializeField]
        private float offsetFactor = 0.1f;

        [SerializeField]
        private float springDuration = 2f;

        [SerializeField]
        private float springFactor = 1f;

        [SerializeField]
        private float springAttenuation = 0.9f;

        private bool _scoreAdded;

        private Vector3 _startPos;
        private Transform _passenger;
        private Vector3 _passengerPos;

        private Coroutine _coroutine;
        private bool _dragging;
        private Vector2 _dragPos;
        private Vector2 _dragPosBegin;

        // 内部状態をリセット
        public void ResetState()
        {
            _scoreAdded = false;

            _startPos = Vector3.zero;
            _passenger = null;
            _passengerPos = Vector3.zero;

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            _dragging = false;
            _dragPos = _dragPosBegin = Vector2.zero;

            anyCollider.enabled = true;
        }

        protected override void Awake()
        {
            base.Awake();
        }

        void Start()
        {
            ResetState();
        }

        protected override void Update()
        {
            base.Update();

            if (!_dragging)
            {
                return;
            }

            // 引っ張った方向にオフセット
            var worldPos = Camera.main.ScreenToWorldPoint(new Vector3(_dragPos.x, _dragPos.y, -Camera.main.transform.position.z));
            var worldPosBegin = Camera.main.ScreenToWorldPoint(new Vector3(_dragPosBegin.x, _dragPosBegin.y, -Camera.main.transform.position.z));

            var diff = worldPos - worldPosBegin;

            var offset = diff.normalized * Mathf.Log(diff.magnitude * offsetFactor + 1, 2);
            Debug.Log($"{offset}");

            transform.position = _startPos + offset;
            _passenger.position = _passengerPos + offset;
        }

        protected override void OnPassengerEnter(Transform passenger)
        {
            _startPos = transform.position;
            _passenger = passenger;
            _passengerPos = _passenger.position;

            BroadcastReceivers.RegisterBroadcastReceiver<ICustomDragEvent>(gameObject);

            Debug.Log($"OnPassengerEnter");

            // スコア加算済みなら以降はスキップ
            if (_scoreAdded)
            {
                return;
            }

            // スコアを加算する
            BroadcastExecuteEvents.Execute<IGameControllerRequests>(null,
                (handler, eventData) => handler.AddScore(score));

            spriteRenderer.color = landedColor;

            _scoreAdded = true;
        }

        protected override void OnPassengerExit(Transform passenger)
        {
            _passenger = null;

            Debug.Log($"OnPassengerExit");

            BroadcastReceivers.UnregisterBroadcastReceiver<ICustomDragEvent>(gameObject);
        }

        protected override void OnPassengerStay(Transform passenger)
        {

        }

        public void OnBeginDrag(Vector2 screenPos)
        {
            _dragging = true;
            _dragPosBegin = screenPos;
        }

        public void OnDrag(Vector2 screenPos, Vector2 beginScreenPos)
        {
            _dragging = true;
            _dragPos = screenPos;
        }

        public void OnEndDrag(Vector2 screenPos)
        {
            _dragging = false;
            _coroutine = StartCoroutine(SpringCoroutine());
        }

        private IEnumerator SpringCoroutine()
        {
            anyCollider.enabled = false;

            Vector3 velocity = Vector3.zero;

            float timer = 0;
            while (timer < springDuration)
            {
                var diff = _startPos - transform.position;
                var acc = diff * springFactor;
                velocity += acc;

                // 減衰
                velocity *= springAttenuation;

                transform.position += velocity * Time.deltaTime;

                yield return null;

                timer += Time.deltaTime;
            }

            transform.position = _startPos;

            anyCollider.enabled = true;
        }

    }
}
