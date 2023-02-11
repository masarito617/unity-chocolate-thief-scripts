using Chocolate.Common;
using Chocolate.Players.Action;
using Chocolate.UIs.View;
using UnityEngine;
using VContainer;

namespace Chocolate.Players.Input.Impl
{
    // モバイル入力用
    // UIから入力を受け取る必要があるため、GameCtrlViewのPresenter的な立ち位置とした
    // （設計めちゃくちゃでかたじけない...）
    public class InputTouchProvider : IInputProvider
    {
        private readonly GameCtrlView _gameCtrlView;
        private StickMoveModel _stickMoveModel;
        private PlayerActionType _pushPlayerAction;

        [Inject]
        public InputTouchProvider(GameCtrlView gameCtrlView)
        {
            _gameCtrlView = gameCtrlView;
        }

        public void OnInitialize(int approachPercent, int pleasePercent, int thiefPercent)
        {
            _stickMoveModel = new StickMoveModel();
            _pushPlayerAction = PlayerActionType.None;

            // Listenerを設定して表示
            _gameCtrlView.SetListenerActionButtons((action) =>
            {
                _pushPlayerAction = action;
            });
            _gameCtrlView.SetTextActionPercentButtons(approachPercent, pleasePercent, thiefPercent);
            _gameCtrlView.AddListenerPointerDownMoveStick(PushDownMoveStickButton);
            _gameCtrlView.AddListenerPointerUpMoveStick(PushUpMoveStickButton);
            _gameCtrlView.SetActive(true);
        }

        // 移動スティック関連
        private const float MoveStickMoveMax = 60.0f;
        private const float MoveStickMoveMin = 10.0f;
        public Vector2 GetMoveVelocity()
        {
            // スティックモデル更新処理
            _stickMoveModel.OnUpdateStick((stickPosition) =>
            {
                _gameCtrlView.SetPositionMoveStickImage(stickPosition);
            });

            // ベロシティ取得
            var moveVelocity = Vector2.zero;
            if (Mathf.Abs(_stickMoveModel.StickDragDiffPosition.x) > MoveStickMoveMin)
            {
                var multiply = _stickMoveModel.StickDragDiffPosition.x < 0 ? -1.0f : 1.0f;
                var dragDiffPosition = Mathf.Abs(_stickMoveModel.StickDragDiffPosition.x);
                moveVelocity.x = ((dragDiffPosition - MoveStickMoveMin) / MoveStickMoveMax) * multiply;
            }
            if (Mathf.Abs(_stickMoveModel.StickDragDiffPosition.y) > MoveStickMoveMin)
            {
                var multiply = _stickMoveModel.StickDragDiffPosition.y < 0 ? -1.0f : 1.0f;
                var dragDiffPosition = Mathf.Abs(_stickMoveModel.StickDragDiffPosition.y);
                moveVelocity.y = ((dragDiffPosition - MoveStickMoveMin) / MoveStickMoveMax) * multiply;
            }
            return moveVelocity;
        }

        private void PushDownMoveStickButton()
        {
            _stickMoveModel.PushDownStick();
        }

        private void PushUpMoveStickButton()
        {
            _stickMoveModel.PushUpStick();
            _gameCtrlView.ResetPositionMoveStickImage();
        }

        // アクションボタン関連
        public PlayerActionType GetPlayerRunAction()
        {
            var tmp = _pushPlayerAction;
            _pushPlayerAction = PlayerActionType.None;
            return tmp;
        }

        public void SetActiveView(bool isActive)
        {
            _gameCtrlView.SetActive(isActive);
        }
    }
}
