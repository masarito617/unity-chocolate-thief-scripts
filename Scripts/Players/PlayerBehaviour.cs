using System.Collections.Generic;
using Chocolate.Girls;
using Chocolate.Players.Action;
using Chocolate.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Chocolate.Players
{
    public class PlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private GameObject playerObject;
        [SerializeField] private PlayerObjectCollider playerObjectCollider;
        [SerializeField] private PlayerActionArea playerActionArea;
        [SerializeField] private PlayerAnimation playerAnimation;
        [SerializeField] private PlayerChocoArea playerChocoArea;
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = gameObject.GetComponent<Rigidbody>();

            // 最初は右向き
            playerObject.transform.localEulerAngles = _rightAngle;
        }

        private bool _isSuccessGetChocolate = false;
        private List<GirlBehaviour> _triggerActiveGirls;
        public PlayerActionType RunAction(PlayerActionType runActionType, GameSettings gameSettings)
        {
            // 女の子と衝突中かつアクション実行された場合
            _triggerActiveGirls = playerActionArea.GetTriggerActiveGirls();
            if (_triggerActiveGirls.Count > 0 && runActionType != PlayerActionType.None)
            {
                // アクション実行時にチョコを消す
                playerChocoArea.SetActiveChocoAreaHand(false);
                playerChocoArea.SetActiveChocoAreaHead(false);

                // パラメータから確率を計算する
                var isSuccess = false;
                if (runActionType == PlayerActionType.Approach) isSuccess = Random.Range(0, 100) < gameSettings.ApproachActionInfo.successPercent;
                if (runActionType == PlayerActionType.Please) isSuccess = Random.Range(0, 100) < gameSettings.PleaseActionInfo.successPercent;
                if (runActionType == PlayerActionType.Thief) isSuccess = Random.Range(0, 100) < gameSettings.ThiefActionInfo.successPercent;

                // アクションを通知する
                _isSuccessGetChocolate = playerActionArea.NotifyActionTriggerGirls(runActionType, isSuccess);

                // 強奪に成功した場合のみ、このタイミングでチョコを表示する
                if (runActionType == PlayerActionType.Thief && _isSuccessGetChocolate)
                {
                    playerChocoArea.SetActiveChocoAreaHand(true);
                }

                return runActionType;
            }
            // 実行できなかったらNoneで返す
            return PlayerActionType.None;
        }

        public bool IsSuccessGetChocolateAndReset()
        {
            var tmp = _isSuccessGetChocolate;
            _isSuccessGetChocolate = false;
            return tmp;
        }

        public int GetSuccessChocoCountScale()
        {
            var scale = 0;
            foreach (var triggerActiveGirl in _triggerActiveGirls)
            {
                scale++;
                if (triggerActiveGirl.IsBonusGirl) scale++; // ボーナスの場合は2倍にする
            }
            return scale;
        }

        public bool IsContainTriggerBonusGirl()
        {
            foreach (var triggerActiveGirl in _triggerActiveGirls)
            {
                if (triggerActiveGirl.IsBonusGirl) return true;
            }
            return false;
        }

        public bool CheckPleaseActionTriggerAngryGirls()
        {
            // 怒り鎮めチェック
            if (playerActionArea.GetTriggerAngryGirlsCount() > 0)
            {
                // アクション実行時にチョコを消す
                playerChocoArea.SetActiveChocoAreaHand(false);
                playerChocoArea.SetActiveChocoAreaHead(false);

                // 怒りを鎮めるよう通知する
                playerActionArea.NotifyActionTriggerAngryGirls();
                _isSuccessGetChocolate = false; // チョコ結果はNGとしておく
                return true;
            }
            return false;
        }

        public void StartGetChocoAnimation()
        {
            // 喜んで頭上にチョコを表示
            playerAnimation.SetHappyTrigger();
            playerChocoArea.ShowHeadChocoArea();
        }

        public void StartDeadAnimation()
        {
            playerAnimation.SetDeadTrigger();
        }

        public bool IsTriggerAngryMoveGirl()
        {
            return playerObjectCollider.IsTriggerAngryMoveGirl();
        }

        private readonly Vector3 _rightAngle = new Vector3(0.0f, 90.0f, 0.0f);
        private readonly Vector3 _leftAngle = new Vector3(0.0f, -90.0f, 0.0f);
        public void UpdateMoveVelocity(Vector2 moveVelocity, float moveSpeed)
        {
            // 移動と向き変更
            if (moveVelocity.magnitude < Mathf.Epsilon)
            {
                _rigidbody.velocity = Vector3.zero;
                playerAnimation.SetMoveVelocity(0.0f);
                return;
            }
            moveVelocity *= moveSpeed; // 移動速度を加算
            _rigidbody.velocity = new Vector3(moveVelocity.x, 0.0f, moveVelocity.y);
            playerObject.transform.localEulerAngles = moveVelocity.x >= 0 ? _rightAngle : _leftAngle;
            playerAnimation.SetMoveVelocity(moveVelocity.magnitude);
        }

        public async UniTask SlideMoveTask()
        {
            // Rigidbodyの空気抵抗(Drag)=4 との組み合わせ
            var addVelocity = 1850.0f * playerObject.transform.forward;
            _rigidbody.AddForce(addVelocity);

            await UniTask.Delay(100);
            _rigidbody.velocity = Vector3.zero;
        }

        // プレイヤーが領域からはみ出さないよう調整
        public void ForceAdjustPlayerPosition()
        {
            // マジックナンバーだけど許して...
            var maxPosX = 7.2f;
            var minPosX = -maxPosX;
            var maxPosZ = 4.7f;
            var minPosZ = -maxPosZ;

            var position = transform.localPosition;
            if (position.x >= maxPosX) position.x = maxPosX;
            if (position.x <= minPosX) position.x = minPosX;
            if (position.z >= maxPosZ) position.z = maxPosZ;
            if (position.z <= minPosZ) position.z = minPosZ;
            transform.localPosition = position;
        }

        public void SetActionTrigger(PlayerActionType actionType)
        {
            playerAnimation.SetActionTrigger(actionType);
        }
    }
}
