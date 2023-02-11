using Chocolate.Players.Action;
using UnityEngine;

namespace Chocolate.Players
{
    public class TitlePlayerBehaviour : MonoBehaviour
    {
        [SerializeField] private PlayerAnimation playerAnimation;
        public void StartTuneUpAnimation()
        {
            playerAnimation.SetActionTrigger(PlayerActionType.Thief);
        }
    }
}
