using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Chocolate.Players
{
    public class PlayerChocoArea : MonoBehaviour
    {
        [SerializeField] private GameObject chocoAreaHand;
        [SerializeField] private GameObject chocoAreaHead;

        private CancellationTokenSource _cancellationTokenSource;

        private void Awake()
        {
            chocoAreaHand.SetActive(false);
            chocoAreaHead.SetActive(false);

            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void SetActiveChocoAreaHand(bool isActive)
        {
            chocoAreaHand.SetActive(isActive);
        }

        public void SetActiveChocoAreaHead(bool isActive)
        {
            chocoAreaHead.SetActive(isActive);
        }

        public void ShowHeadChocoArea()
        {
            ShowHeadChocoAreaTask(_cancellationTokenSource.Token);
        }

        private async UniTask ShowHeadChocoAreaTask(CancellationToken token)
        {
            await UniTask.Delay(300, cancellationToken: token);

            // 頭上のチョコ表示
            chocoAreaHand.SetActive(false);
            chocoAreaHead.SetActive(true);

            // 回転させる
            var rotation = chocoAreaHead.transform.localRotation;
            chocoAreaHead.transform.DOLocalRotate(new Vector3(0, 360.0f, 0.0f), 1.0f, RotateMode.FastBeyond360);
            await UniTask.Delay(1000, cancellationToken: token);
            chocoAreaHead.SetActive(false);
            await UniTask.Delay(200, cancellationToken: token);
            chocoAreaHead.transform.localRotation = rotation;
        }
    }
}
