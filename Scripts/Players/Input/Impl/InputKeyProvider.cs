using Chocolate.Players.Action;
using UnityEngine;

namespace Chocolate.Players.Input.Impl
{
    // Editorでのテスト入力用
    public class InputKeyProvider : IInputProvider
    {
        public void OnInitialize(int approachPercent, int pleasePercent, int thiefPercent)
        {
        }

        public Vector2 GetMoveVelocity()
        {
            var moveVelocity = Vector2.zero;
            if (UnityEngine.Input.GetKey(KeyCode.A)) moveVelocity.x -= 1.0f;
            if (UnityEngine.Input.GetKey(KeyCode.D)) moveVelocity.x += 1.0f;
            if (UnityEngine.Input.GetKey(KeyCode.S)) moveVelocity.y -= 1.0f;
            if (UnityEngine.Input.GetKey(KeyCode.W)) moveVelocity.y += 1.0f;
            return moveVelocity;
        }

        public PlayerActionType GetPlayerRunAction()
        {
            if (UnityEngine.Input.GetKey(KeyCode.J)) return PlayerActionType.Approach;
            if (UnityEngine.Input.GetKey(KeyCode.K)) return PlayerActionType.Please;
            if (UnityEngine.Input.GetKey(KeyCode.L)) return PlayerActionType.Thief;
            return PlayerActionType.None;
        }

        public void SetActiveView(bool isActive)
        {
        }
    }
}
