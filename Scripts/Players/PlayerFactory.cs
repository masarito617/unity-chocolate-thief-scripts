using Chocolate.Data;
using Chocolate.Data.Event;
using Chocolate.Players.Input;
using Chocolate.Services;
using Chocolate.Settings;
using MessagePipe;

namespace Chocolate.Players
{
    public class PlayerFactory
    {
        private readonly IAudioService _audioService;
        private readonly IAssetsService _assetsService;
        private readonly IInputProvider _inputProvider;
        private readonly IPublisher<ScoreData> _onSendScoreEvent;
        private readonly IPublisher<DoActionData> _doPlayerActionEvent;
        private readonly GameSettings _gameSettings;
        public PlayerFactory(IAudioService audioService, IAssetsService assetsService, IInputProvider inputProvider, IPublisher<ScoreData> onSendScoreEvent, IPublisher<DoActionData> doPlayerActionEvent, GameSettings gameSettings)
        {
            _audioService = audioService;
            _assetsService = assetsService;
            _inputProvider = inputProvider;
            _onSendScoreEvent = onSendScoreEvent;
            _doPlayerActionEvent = doPlayerActionEvent;
            _gameSettings = gameSettings;
        }

        public Player CreatePlayer()
        {
            var player = new Players.Player(_assetsService.LoadAssets("Player"), _inputProvider, _onSendScoreEvent, _doPlayerActionEvent, _audioService, _gameSettings);
            return player;
        }
    }
}
