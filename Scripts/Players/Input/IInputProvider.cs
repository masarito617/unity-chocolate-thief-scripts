using Chocolate.Players.Action;
using UnityEngine;

namespace Chocolate.Players.Input
{
    public interface IInputProvider
    {
        public void OnInitialize(int approachPercent, int pleasePercent, int thiefPercent);
        public Vector2 GetMoveVelocity();
        public PlayerActionType GetPlayerRunAction();
        public void SetActiveView(bool isActive);
    }
}
