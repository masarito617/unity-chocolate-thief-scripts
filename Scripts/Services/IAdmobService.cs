using UnityEngine.Events;

namespace Chocolate.Services
{
    public interface IAdmobService
    {
        public void ShowBanner();
        public void HideBanner();
        public void ShowInterstitial(UnityAction callback);
        public void DestroyInterstitialAd();

    }
}