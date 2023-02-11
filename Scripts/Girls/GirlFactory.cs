using Chocolate.Services;
using Chocolate.Settings;

namespace Chocolate.Girls
{
    public class GirlFactory
    {
        private readonly IAssetsService _assetsService;
        private readonly IAudioService _audioService;
        private readonly GameSettings _gameSettings;
        public GirlFactory(IAssetsService assetsService, IAudioService audioService, GameSettings gameSettings)
        {
            _assetsService = assetsService;
            _audioService = audioService;
            _gameSettings = gameSettings;
        }

        public Girl CreateGirl()
        {
            var girl = new Girl(_assetsService.LoadAssets("Girl"), _audioService, _gameSettings);
            return girl;
        }
    }
}
