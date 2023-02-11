using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Chocolate.UIs.View
{
    public class TitleHelpView : MonoBehaviour
    {
        /// <summary>
        /// ヘルプ内容
        /// </summary>
        [Serializable]
        public class HelpPageInfo
        {
            public TextMeshProUGUI titleText;
            public Image image;
            public TextMeshProUGUI detailText;
        }
        [SerializeField] private List<HelpPageInfo> helpPageInfoList;
        private void ShowHelpPageInfo(int index)
        {
            var helpPageInfo = helpPageInfoList[index];
            helpPageInfo.titleText.gameObject.SetActive(true);
            helpPageInfo.image.gameObject.SetActive(true);
            helpPageInfo.detailText.gameObject.SetActive(true);
        }
        private void HideAllHelpPageInfo()
        {
            foreach (var helpPageInfo in helpPageInfoList)
            {
                helpPageInfo.titleText.gameObject.SetActive(false);
                helpPageInfo.image.gameObject.SetActive(false);
                helpPageInfo.detailText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ウィンドウ
        /// </summary>
        [SerializeField] private GameObject helpWindow;
        [SerializeField] private Button okButton;
        [SerializeField] private Button nextButton;

        public void ShowHelpWindow(UnityAction nextCallback, UnityAction closeCallback)
        {
            // 再帰的にコールバックを設定
            var index = 0;
            SetRecursiveListenerButton(nextCallback, closeCallback, index);
            // ヘルプウィンドウ表示
            helpWindow.transform.localScale = Vector3.zero; // ウィンドウは初めは非表示
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() => gameObject.SetActive(true));
            sequence.Append(helpWindow.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutExpo));
        }

        private void SetRecursiveListenerButton(UnityAction nextCallback, UnityAction closeCallback, int index)
        {
            HideAllHelpPageInfo();
            ShowHelpPageInfo(index);
            if (index >= helpPageInfoList.Count - 1)
            {
                // 最後のページの場合
                nextButton.gameObject.SetActive(false);
                okButton.onClick.RemoveAllListeners();
                okButton.onClick.AddListener(() =>
                {
                    closeCallback?.Invoke();
                    var sequence = DOTween.Sequence();
                    sequence.Append(helpWindow.transform.DOScale(0f, 0.3f).SetEase(Ease.OutQuart));
                    sequence.AppendCallback(() => gameObject.SetActive(false));
                });
                okButton.gameObject.SetActive(true);
            }
            else
            {
                // 次のページがある場合
                okButton.gameObject.SetActive(false);
                nextButton.onClick.RemoveAllListeners();
                nextButton.onClick.AddListener(() =>
                {
                    nextCallback?.Invoke();
                    // インデックスを加算して再帰的にコールバックを設定
                    index++;
                    SetRecursiveListenerButton(nextCallback, closeCallback, index);
                });
                nextButton.gameObject.SetActive(true);
            }
        }
    }
}
