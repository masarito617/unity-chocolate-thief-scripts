using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Chocolate.UIs.View
{
    public class RewardView : MonoBehaviour
    {
        [SerializeField] private Image rewardImage;
        public void StartRewardImageAnimation()
        {
            rewardImage.transform.DOLocalMoveY(20f, 0.4f)
                .SetRelative(true)
                .SetEase(Ease.OutQuad)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}
