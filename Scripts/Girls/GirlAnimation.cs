using UnityEngine;

namespace Chocolate.Girls
{
    public class GirlAnimation : MonoBehaviour
    {
        /// <summary>
        /// アニメーション関連
        /// </summary>
        private Animator _animator;
        private static readonly string AnimParamVelocity = "VelocityFloat";
        private static readonly string AnimParamTake = "TakeTrigger";
        private static readonly string AnimParamBool = "AngryBool";

        private void Awake()
        {
            _animator = gameObject.GetComponent<Animator>();
        }

        public void SetMoveVelocity(float velocity)
        {
            _animator.SetFloat(AnimParamVelocity, velocity);
        }

        public void SetTakeTrigger()
        {
            _animator.SetTrigger(AnimParamTake);
        }

        public void SetAngryBool(bool isAngry)
        {
            _animator.SetBool(AnimParamBool, isAngry);
        }
    }
}
