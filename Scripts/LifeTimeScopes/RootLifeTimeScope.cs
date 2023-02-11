using Chocolate.Common;
using Chocolate.Data;
using Chocolate.Services;
using Chocolate.Services.Impl;
using Chocolate.Settings;
using Chocolate.UIs.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Chocolate.LifeTimeScopes
{
    public class RootLifeTimeScope : LifetimeScope
    {
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private AdmobSettings admobSettings;
        [SerializeField] private GameObject transitionViewPrefab;
        [SerializeField] private bool isForceMobile = false; // Editorでモバイルの挙動をテストしたい場合、ここをtrueにする

        protected override void Configure(IContainerBuilder builder)
        {
            // モバイルプラットフォームか？
            if (Application.isMobilePlatform || isForceMobile)
            {
                gameSettings.IsMobilePlatform = true;
            }

            // 遷移用View作成
            var transitionView = Instantiate(transitionViewPrefab).GetComponent<TransitionView>();
            transitionView.gameObject.AddComponent<DontDestroyBehaviour>();
            transitionView.SetActive(false); // 最初は非表示
            builder.RegisterComponent(transitionView);

            // Repository
            builder.Register<PlayerPrefsRepository>(Lifetime.Singleton);

            // Services
            builder.Register<IAdmobService, AdmobService>(Lifetime.Singleton);
            builder.Register<IAudioService, AudioService>(Lifetime.Singleton);
            builder.Register<IAssetsService, AssetsService>(Lifetime.Singleton);
            builder.Register<IStoreReviewService, StoreReviewService>(Lifetime.Singleton);
            builder.Register<ITransitionService, TransitionService>(Lifetime.Singleton);
            builder.Register<IPlayerPrefsService, PlayerPrefsService>(Lifetime.Singleton);
            builder.Register<ISocialService, SocialService>(Lifetime.Singleton);

            // Instances
            builder.RegisterInstance(gameSettings);
            builder.RegisterInstance(admobSettings);
        }
    }
}
