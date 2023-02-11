using Chocolate.Const;
using Chocolate.Players.Action;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chocolate.Players
{
    public class PlayerAnimation : MonoBehaviour
    {
        /// <summary>
        /// アニメーション関連
        /// </summary>
        private Animator _animator;
        private static readonly string AnimParamVelocity = "VelocityFloat";
        private static readonly string AnimParamApproach = "ApproachTrigger";
        private static readonly string AnimParamPlease = "PleaseTrigger";
        private static readonly string AnimParamThief = "ThiefTrigger";
        private static readonly string AnimParamHappy = "HappyTrigger";
        private static readonly string AnimParamDead = "DeadTrigger";

        private void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        public void SetMoveVelocity(float velocity)
        {
            _animator.SetFloat(AnimParamVelocity, velocity);
        }

        public void SetActionTrigger(PlayerActionType actionType)
        {
            switch (actionType)
            {
                case PlayerActionType.Approach:
                    _animator.SetTrigger(AnimParamApproach);
                    break;
                case PlayerActionType.Please:
                    _animator.SetTrigger(AnimParamPlease);
                    break;
                case PlayerActionType.Thief:
                    _animator.SetTrigger(AnimParamThief);
                    break;
            }
        }

        public void SetHappyTrigger()
        {
            _animator.SetTrigger(AnimParamHappy);
        }

        public void SetDeadTrigger()
        {
            _animator.SetTrigger(AnimParamDead);
        }
    }
}
