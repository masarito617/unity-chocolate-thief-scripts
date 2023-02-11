using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Chocolate.UIs.View
{
    public class TitleBoostView : MonoBehaviour
    {
        [SerializeField] private GameObject boostWindow;
        [SerializeField] private Button okButton;
        [SerializeField] private Button cancelButton;
        public void ShowBoostWindow()
        {
            boostWindow.transform.localScale = Vector3.zero; // ウィンドウは初めは非表示
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() => gameObject.SetActive(true));
            sequence.Append(boostWindow.transform.DOScale(1.0f, 0.3f).SetEase(Ease.OutExpo));
        }
        public void SetListenerOkButton(UnityAction action)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(action);
        }
        public void SetListenerCancelButton(UnityAction action)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(action);
        }
    }
}
