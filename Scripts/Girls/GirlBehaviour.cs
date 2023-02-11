using Chocolate.Const;
using Chocolate.Players.Action;
using Chocolate.Roads;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Chocolate.Girls
{
    public class GirlBehaviour : MonoBehaviour
    {
        private static readonly float MinMoveSpeed = 1.5f;
        private static readonly float MaxMoveSpeed = 3.0f;
        private float _moveSpeed = 0.0f;

        private static readonly float OffsetPosition = 0.2f;

        [SerializeField] private GameObject girlObject;
        [SerializeField] private GirlAnimation girlAnimation;
        [SerializeField] private GirlChocoArea girlChocoArea;
        [SerializeField] private SkinnedMeshRenderer girlMeshRenderer;
        [SerializeField] private Material girlMaterial;
        [SerializeField] private Material girlAngryMaterial;
        [SerializeField] private Material girlGoldMaterial;
        [SerializeField] private Material girlGoldAngryMaterial;
        [SerializeField] private GirlMarkerBehaviour girlMarkerBehaviour;
        private Rigidbody _rigidbody;

        private Material _standardMaterial;
        private Material _angryMaterial;

        private Vector3 _startPosition;
        private Vector3 _endPosition;

        public int girlId; // 一意なID（衝突判定用）

        public bool IsBonusGirl { get; private set; } // ボーナスキャラ（スコア2倍）

        private void Awake()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();
        }

        public void HideGirl()
        {
            gameObject.SetActive(false);
        }

        public void ShowGirl(RoadPassesBehaviour.RoadPass roadPass, bool isBonus)
        {
            // ボーナス設定
            IsBonusGirl = isBonus;
            _standardMaterial = isBonus ? girlGoldMaterial : girlMaterial;
            _angryMaterial = isBonus ? girlGoldAngryMaterial : girlAngryMaterial;
            girlMeshRenderer.material = _standardMaterial;

            // 開始位置、終了位置の設定
            // 反転させたいばらつきを持たせる
            var isReverse = Random.Range(0, 100) < 50;
            _startPosition = isReverse ? roadPass.endPosition.position : roadPass.startPosition.position;
            _endPosition = isReverse ? roadPass.startPosition.position : roadPass.endPosition.position;
            var offsetX = Random.Range(-OffsetPosition, OffsetPosition);
            var offsetY = Random.Range(-OffsetPosition, OffsetPosition);
            _startPosition.x += offsetX;
            _startPosition.y += offsetY;
            _endPosition.x += offsetX;
            _endPosition.y += offsetY;

            gameObject.transform.position = _startPosition;
            transform.LookAt(_endPosition, Vector3.up);
            gameObject.SetActive(true);

            // 歩行スピードはランダムに決める
            _moveSpeed = Random.Range(MinMoveSpeed, MaxMoveSpeed);

            // 右手か左手どちらかランダムに持たせる
            var isLeft = Random.Range(0, 100) < 50;
            if (isLeft)
            {
                girlChocoArea.SetActiveChocoAreaL(true);
                girlChocoArea.SetActiveChocoAreaR(false);
            }
            else
            {
                girlChocoArea.SetActiveChocoAreaL(false);
                girlChocoArea.SetActiveChocoAreaR(true);
            }

            // アクションフラグをリセット
            _receiveActionInfo = null;
            _isEvenTakeAction = false;
        }

        public void UpdateMoveTarget()
        {
            transform.position = Vector3.MoveTowards(transform.position, _endPosition, _moveSpeed * Time.deltaTime);
            transform.LookAt(_endPosition, Vector3.up);
            // アニメーション値の設定
            girlAnimation.SetMoveVelocity(_moveSpeed);
        }

        public void UpdateGirlMarker()
        {
            girlMarkerBehaviour.SetTriggerMaterial(_isPlayerActionTrigger);
        }

        public void SetActiveGirlMarker(bool isActive)
        {
            girlMarkerBehaviour.SetActive(isActive);
        }

        public bool IsArriveTarget()
        {
            return Vector3.Distance(transform.position, _endPosition) < 0.1f;
        }

        public void LookAtPlayer(GameObject player)
        {
            if (player == null) return;
            transform.LookAt(player.transform, Vector3.up);
            // アニメーション値の設定
            girlAnimation.SetMoveVelocity(0.0f);
        }

        private bool _isAngryMove = false;
        public bool IsAngryMove()
        {
            return _isAngryMove;
        }

        /// <summary>
        /// 怒り関連
        /// </summary>
        public void StartAngry()
        {
            girlMeshRenderer.material = _angryMaterial;
            girlAnimation.SetAngryBool(true);
        }
        public void UpdateAngryMove(GameObject player)
        {
            _isAngryMove = true;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, _moveSpeed * 0.2f * Time.deltaTime);
            transform.LookAt(player.transform, Vector3.up);
            // アニメーション値の設定
            girlAnimation.SetMoveVelocity(_moveSpeed);
        }
        public void EndAngry()
        {
            girlMeshRenderer.material = _standardMaterial;
            _isAngryMove = false;
            girlAnimation.SetAngryBool(false);
        }

        /// <summary>
        /// 一度でもアクションを受けたか
        /// </summary>
        private bool _isEvenTakeAction = false;
        public bool IsEvenTakeAction()
        {
            return _isEvenTakeAction;
        }

        /// <summary>
        /// プレイヤーアクションエリアのトリガー中か？
        /// </summary>
        private bool _isPlayerActionTrigger = false;
        public void SetIsPlayerActionTrigger(bool isActive)
        {
            _isPlayerActionTrigger = isActive;
        }

        private class ReceiveActionInfo
        {
            public PlayerActionType ReceiveAction;
            public bool IsSuccess;
            public ReceiveActionInfo(PlayerActionType receiveAction, bool isSuccess)
            {
                ReceiveAction = receiveAction;
                IsSuccess = isSuccess;
            }
        }
        private ReceiveActionInfo _receiveActionInfo;

        /// <summary>
        /// プレイヤーからのアクション通知
        /// PlayerActionArea.cs から呼ばれる
        /// </summary>
        /// <param name="playerActionType"></param>
        /// <param name="isSuccess"></param>
        public void ReceivePlayerAction(PlayerActionType playerActionType, bool isSuccess)
        {
            if (_isEvenTakeAction) return;
            _isEvenTakeAction = true;
            _receiveActionInfo = new ReceiveActionInfo(playerActionType, isSuccess);

            // 強奪に成功した場合のみ、このタイミングでチョコを非表示にする
            if (playerActionType == PlayerActionType.Thief && isSuccess)
            {
                girlChocoArea.SetActiveChocoAreaL(false);
                girlChocoArea.SetActiveChocoAreaR(false);
            }
        }

        /// <summary>
        /// プレイヤーからのアクション通知（怒りを鎮める）
        /// </summary>
        public void ReceiveDownAngry()
        {
            isForceDownAngry = true;
        }
        public bool isForceDownAngry;

        public bool IsReceivePlayerAction()
        {
            return _receiveActionInfo != null;
        }

        public void StartTakeAnimation()
        {
            if (_receiveActionInfo != null && _receiveActionInfo.IsSuccess
                && _receiveActionInfo.ReceiveAction != PlayerActionType.Thief) // 強奪時は除く
            {
                girlAnimation.SetTakeTrigger();
            }
        }

        public GirlState GetCheckResultGirlState()
        {
            // 強奪された場合、怒る
            if (_receiveActionInfo.IsSuccess && _receiveActionInfo.ReceiveAction == PlayerActionType.Thief
                && Random.Range(0, 100) < 50) // とりあえず5割
            {
                return GirlState.Angry;
            }
            return GirlState.Walk;
        }

        public void ResetReceivePlayerAction(bool isDownAngry = false)
        {
            // 成功していたらチョコを非表示にする
            if (_receiveActionInfo != null && _receiveActionInfo.IsSuccess)
            {
                girlChocoArea.SetActiveChocoAreaL(false);
                girlChocoArea.SetActiveChocoAreaR(false);
            }
            _receiveActionInfo = null;
            SetEscapeInfo(isDownAngry); // 逃げるための設定
        }

        private void SetEscapeInfo(bool isDownAngry)
        {
            _moveSpeed = isDownAngry ? 3.0f : 6.0f; // 走らせる

            var player = GameObject.FindWithTag(GameConst.TagNamePlayer); // めんどくさいのでFindしてしまう
            var playerNormVec = (transform.position - player.transform.position).normalized;
            var startNormVec = (_startPosition - transform.position).normalized;
            var endNormVec = (_endPosition - transform.position).normalized;

            // プレイヤーから女の子の向きの内積に近い方を走る方向にする
            var dotStartVec = Vector3.Dot(playerNormVec, startNormVec);
            var dotEndVec = Vector3.Dot(playerNormVec, endNormVec);
            if (dotStartVec > dotEndVec)
            {
                (_startPosition, _endPosition) = (_endPosition, _startPosition);
            }
        }
    }
}
