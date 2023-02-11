using Chocolate.Services;

namespace Chocolate.Girls
{
    public class GirlFactory
    {
        private readonly IAssetsService _assetsService;
        private readonly IAudioService _audioService;
        public GirlFactory(IAssetsService assetsService, IAudioService audioService)
        {
            _assetsService = assetsService;
            _audioService = audioService;
        }

        public Girl CreateGirl()
        {
            var girl = new Girl(_assetsService.LoadAssets("Girl"), _audioService);
            return girl;
        }
    }
}
