using UnityEngine;
using UnityEngine.Events;

namespace Chocolate.Common
{
    public class StickMoveModel
    {
        private const float StickMoveRadiusMax = 80.0f; // 最大移動半径
        private const float StickMoveRadiusMin = 40.0f; // 最小移動半径

        private Touch _stickTouch;                // タッチ情報
        private Vector3 _stickBeganTouchPosition; // タッチ開始時の位置

        public bool IsPushStick { get; private set; } // 押下されているか？
        public Vector3 StickDragDiffPosition { get; private set; } // スティック移動量

        public StickMoveModel()
        {
            // タッチ情報を初期化
            IsPushStick = false;
            _stickTouch = TouchUtil.NotTouch;
            StickDragDiffPosition = Vector3.zero;
            _stickBeganTouchPosition = Vector3.zero;
        }

        /// <summary>
        /// スティック情報更新処理
        /// </summary>
        /// <param name="moveStickAction">スティックを動かす処理</param>
        /// <param name="canvasRectTransform">キャンバスRectTransform</param>
        public void OnUpdateStick(UnityAction<Vector3> moveStickAction = null, RectTransform canvasRectTransform = null)
        {
            if (!IsPushStick) return;

            // タップ座標、ドラッグ座標を設定
            var screenTapPosition = _stickBeganTouchPosition;
            var screenDragPosition = TouchUtil.GetCurrentTouchPosition(_stickTouch);

            // ドラッグ差分を算出
            var canvasDiffPosition = TouchUtil.CalculateDragDiffPosition(
                screenTapPosition, screenDragPosition,
                canvasRectTransform); // 解像度考慮のためキャンバス座標で取得
            var canvasDiffMagnitude = canvasDiffPosition.magnitude;

            // 最大半径より大きいなら丸める
            if (StickMoveRadiusMax < canvasDiffMagnitude)
            {
                canvasDiffPosition.Normalize();
                canvasDiffPosition *= StickMoveRadiusMax;
            }

            // 最小半径より大きいなら移動量を設定
            StickDragDiffPosition = Vector3.zero;
            if (StickMoveRadiusMin < canvasDiffMagnitude)
            {
                StickDragDiffPosition = canvasDiffPosition;
            }

            // スティックを動かす処理
            if (moveStickAction != null)
            {
                var canvasTapPosition = canvasRectTransform != null ? TouchUtil.ScreenToCanvas(screenTapPosition, canvasRectTransform) : screenTapPosition;
                var stickPosition = canvasTapPosition + canvasDiffPosition;
                moveStickAction.Invoke(stickPosition);
            }
        }

        /// <summary>
        /// スティックを押下した時
        /// </summary>
        public void PushDownStick()
        {
            // タッチ情報を設定
            IsPushStick = true;
            _stickTouch = TouchUtil.GetBeganTouch();
            _stickBeganTouchPosition = _stickTouch.position;
        }

        /// <summary>
        /// スティックを離した時
        /// </summary>
        public void PushUpStick()
        {
            // タッチ情報を初期化
            IsPushStick = false;
            _stickTouch = TouchUtil.NotTouch;
            StickDragDiffPosition = Vector3.zero;
            _stickBeganTouchPosition = Vector3.zero;
        }
    }
}
