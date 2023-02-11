using UnityEngine;

namespace Chocolate.Settings
{
    [CreateAssetMenu(fileName = "AdmobSettings", menuName = "Chocolate/AdmobSettings")]
    public class AdmobSettings : ScriptableObject
    {
        /// <summary>
        /// バナー広告ID
        /// </summary>
        public string debugAndroidBannerUnitId;
        public string debugIosBannerUnitId;
        public string releaseAndroidBannerUnitId;
        public string releaseIosBannerUnitId;

        /// <summary>
        /// インタースティシャル広告ID
        /// </summary>
        public string debugAndroidInterstitialUnitId;
        public string debugIosInterstitialUnitId;
        public string releaseAndroidInterstitialUnitId;
        public string releaseIosInterstitialUnitId;
    }
}