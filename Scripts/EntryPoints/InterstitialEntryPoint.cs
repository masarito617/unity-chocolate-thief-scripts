using System;
using Chocolate.Const;
using Chocolate.Services;
using VContainer;
using VContainer.Unity;

namespace Chocolate.EntryPoints
{
    /// <summary>
    /// インタースティシャルSceneエントリーポイント
    /// </summary>
    public class InterstitialEntryPoint : IStartable, IDisposable
    {
        private readonly IAdmobService _admobService;
        private readonly IAudioService _audioService;
        private readonly ITransitionService _transitionService;

        [Inject]
        public InterstitialEntryPoint(IAdmobService admobService, IAudioService audioService, ITransitionService transitionService)
        {
            _admobService = admobService;
            _audioService = audioService;
            _transitionService = transitionService;
        }

        public void Start()
        {
            // BGMを停止
            _audioService.StopBGM();

            // インタースティシャル広告再生
            _admobService.ShowInterstitial(() =>
            {
                // 広告再生後はタイトルへ遷移
                _transitionService.LoadScene(GameConst.SceneNameTitle);
            });
        }

        public void Dispose()
        {
            // インタースティシャル広告破棄
            _admobService.DestroyInterstitialAd();
        }
    }
}