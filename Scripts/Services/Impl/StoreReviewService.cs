namespace Chocolate.Services.Impl
{
    public class StoreReviewService : IStoreReviewService
    {
        public void ShowStoreReview()
        {
            // 時間がないためiOSのみ実装
#if UNITY_IOS && !UNITY_EDITOR
            UnityEngine.iOS.Device.RequestStoreReview();
#else
            UnityEngine.Debug.Log("Not Support Request Store Review.");
#endif
        }
    }
}
