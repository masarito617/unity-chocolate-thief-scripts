using Chocolate.Players.Action;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Chocolate.UIs.View
{
    public class GameCtrlView : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
            // 初期位置を保持しておく(Awakeのタイミングだとだめっぽい)
            _moveStickInitPosition = moveStickImage.transform.position;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// 移動スティック関連
        /// </summary>
        [SerializeField] private EventTrigger moveStickEventTrigger;
        public void AddListenerPointerDownMoveStick(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(call => action());
            moveStickEventTrigger.triggers.Add(entry);
        }
        public void AddListenerPointerUpMoveStick(UnityAction action)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener(call => action());
            moveStickEventTrigger.triggers.Add(entry);
        }

        [SerializeField] private Image moveStickImage;
        private Vector3 _moveStickInitPosition;
        public void SetPositionMoveStickImage(Vector3 position)
        {
            moveStickImage.transform.position = position;
        }
        public void ResetPositionMoveStickImage()
        {
            moveStickImage.transform.position = _moveStickInitPosition;
        }

        /// <summary>
        /// アクションボタン
        /// </summary>
        [SerializeField] private Button approachButton;
        [SerializeField] private TextMeshProUGUI approachPercentText;
        [SerializeField] private Button pleaseButton;
        [SerializeField] private TextMeshProUGUI pleasePercentText;
        [SerializeField] private Button thiefButton;
        [SerializeField] private TextMeshProUGUI thiefPercentText;
        public void SetListenerActionButtons(UnityAction<PlayerActionType> action)
        {
            approachButton.onClick.RemoveAllListeners();
            approachButton.onClick.AddListener(() => action(PlayerActionType.Approach));
            pleaseButton.onClick.RemoveAllListeners();
            pleaseButton.onClick.AddListener(() => action(PlayerActionType.Please));
            thiefButton.onClick.RemoveAllListeners();
            thiefButton.onClick.AddListener(() => action(PlayerActionType.Thief));
        }
        public void SetTextActionPercentButtons(int approachPercent, int pleasePercent, int thiefPercent)
        {
            approachPercentText.text = approachPercent + "%";
            pleasePercentText.text = pleasePercent + "%";
            thiefPercentText.text = thiefPercent + "%";
        }
    }
}
