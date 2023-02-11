using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Chocolate.UIs.View
{
    public class TitleTuneUpView : MonoBehaviour
    {
        // HELPボタン
        [SerializeField] private Button helpButton;
        public void SetListenerHelpButton(UnityAction action)
        {
            helpButton.onClick.RemoveAllListeners();
            helpButton.onClick.AddListener(action);
        }

        // GOボタン
        [SerializeField] private Button goButton;
        public void SetActiveGoButton(bool isActive)
        {
            goButton.gameObject.SetActive(isActive);
        }
        public void SetListenerGoButton(UnityAction action)
        {
            goButton.onClick.RemoveAllListeners();
            goButton.onClick.AddListener(action);
        }

        // 強化ボタン
        [SerializeField] private Button tuneButton;
        public void SetActiveTuneButton(bool isActive)
        {
            tuneButton.gameObject.SetActive(isActive);
        }
        public void SetListenerTuneButton(UnityAction action)
        {
            tuneButton.onClick.RemoveAllListeners();
            tuneButton.onClick.AddListener(action);
        }

        // リセットボタン
        [SerializeField] private Button statusResetButton;
        public void SetListenerStatusResetButton(UnityAction action)
        {
            statusResetButton.onClick.RemoveAllListeners();
            statusResetButton.onClick.AddListener(action);
        }

        // BACKボタン
        [SerializeField] private Button backButton;
        public void SetListenerBackButton(UnityAction action)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(action);
        }

        // エフェクト描画オプション
        [SerializeField] private Toggle effectOptionToggle;
        public void SetIsOnEffectOptionToggle(bool isOn)
        {
            effectOptionToggle.isOn = isOn;
        }
        public void SetListenerEffectOptionToggle(UnityAction<bool> action)
        {
            effectOptionToggle.onValueChanged.RemoveAllListeners();
            effectOptionToggle.onValueChanged.AddListener(action);
        }

        // EXPブーストオプション
        [SerializeField] private GameObject boostOptionArea;
        [SerializeField] private Toggle boostOptionToggle;
        public void SetActiveBoostOptionArea(bool isActive)
        {
            boostOptionArea.SetActive(isActive);
        }
        public void SetIsOnBoostOptionToggle(bool isOn)
        {
            boostOptionToggle.isOn = isOn;
        }
        public void SetIsDisabledBoostOptionToggle(bool isDisabled)
        {
            boostOptionToggle.interactable = !isDisabled;
        }
        public void SetListenerBoostOptionToggle(UnityAction<bool> action)
        {
            boostOptionToggle.onValueChanged.RemoveAllListeners();
            boostOptionToggle.onValueChanged.AddListener(action);
        }

        // プレイヤーステータス
        // EXP
        [SerializeField] private TextMeshProUGUI playerStatusExpText;
        public void SetTextPlayerStatusExp(int value)
        {
            playerStatusExpText.text = value.ToString();
        }
        // 魅力
        [SerializeField] private TextMeshProUGUI playerStatusCharmText;
        [SerializeField] private Button playerStatusCharmPlusButton;
        public void SetTextPlayerStatusCharm(int value, bool isMax)
        {
            playerStatusCharmText.text = isMax ? "MAX" : value.ToString();
        }
        public void SetListenerPlayerStatusCharmPlusButton(UnityAction action)
        {
            playerStatusCharmPlusButton.onClick.RemoveAllListeners();
            playerStatusCharmPlusButton.onClick.AddListener(action);
        }
        // テクニック
        [SerializeField] private TextMeshProUGUI playerStatusTechText;
        [SerializeField] private Button playerStatusTechPlusButton;
        public void SetTextPlayerStatusTech(int value, bool isMax)
        {
            playerStatusTechText.text = isMax ? "MAX" : value.ToString();
        }
        public void SetListenerPlayerStatusTechPlusButton(UnityAction action)
        {
            playerStatusTechPlusButton.onClick.RemoveAllListeners();
            playerStatusTechPlusButton.onClick.AddListener(action);
        }
        // スピード
        [SerializeField] private TextMeshProUGUI playerStatusSpeedText;
        [SerializeField] private Button playerStatusSpeedPlusButton;
        public void SetTextPlayerStatusSpeed(int value, bool isMax)
        {
            playerStatusSpeedText.text = isMax ? "MAX" : value.ToString();
        }
        public void SetListenerPlayerStatusSpeedPlusButton(UnityAction action)
        {
            playerStatusSpeedPlusButton.onClick.RemoveAllListeners();
            playerStatusSpeedPlusButton.onClick.AddListener(action);
        }

        // 累計スコア
        [SerializeField] private TextMeshProUGUI totalScoreText;
        public void SetTextTotalScore(int value)
        {
            totalScoreText.text = value.ToString();
        }

        // ベストスコア
        [SerializeField] private TextMeshProUGUI bestScoreText;
        public void SetTextBestScore(int value)
        {
            bestScoreText.text = value.ToString();
        }

        // 成功確率
        [SerializeField] private TextMeshProUGUI approachPercentText;
        public void SetTextApproachPercent(int value, bool isMax)
        {
            if (isMax)
            {
                approachPercentText.text = "<color=\"blue\">" + value + "%</color>";
                return;
            }
            approachPercentText.text = value + "%";
        }
        [SerializeField] private TextMeshProUGUI pleasePercentText;
        public void SetTextPleasePercent(int value, bool isMax)
        {
            if (isMax)
            {
                pleasePercentText.text = "<color=\"blue\">" + value + "%</color>";
                return;
            }
            pleasePercentText.text = value + "%";
        }
        [SerializeField] private TextMeshProUGUI thiefPercentText;
        public void SetTextThiefPercent(int value, bool isMax)
        {
            if (isMax)
            {
                thiefPercentText.text = "<color=\"blue\">" + value + "%</color>";
                return;
            }
            thiefPercentText.text = value + "%";
        }
        // スピード
        [SerializeField] private TextMeshProUGUI playerSpeedText;
        public void SetTextPlayerSpeed(float value, bool isMax)
        {
            if (isMax)
            {
                playerSpeedText.text = "<color=\"blue\">" + value.ToString("f1") + "</color>";
                return;
            }
            playerSpeedText.text = value.ToString("f1");
        }
    }
}
