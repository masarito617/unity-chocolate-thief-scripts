using System.Threading;
using Chocolate.Audio;
using Chocolate.Data;
using Chocolate.Data.Event;
using Chocolate.Managers;
using Chocolate.Services;
using Chocolate.Settings;
using Chocolate.UIs.View;
using MessagePipe;
using UniRx;
using UnityEngine;
using VContainer;

namespace Chocolate.UIs.Presenter
{
    public class GamePresenter : MonoBehaviour
    {
        [SerializeField] private GameInfoView gameInfoView;
        [SerializeField] private GameResultView gameResultView;
        [SerializeField] private GameEffectView gameEffectView;

        private GameManager _gameManager;
        private ScoreManager _scoreManager;
        private GameSettings _gameSettings;
        private IAudioService _audioService;
        private ISocialService _socialService;
        private PlayerPrefsRepository _playerPrefsRepository;
        private ISubscriber<DoActionData> _receiveActionData;
        private CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public void Construct(GameManager gameManager, ScoreManager scoreManager, GameSettings gameSettings, IAudioService audioService, ISocialService socialService, PlayerPrefsRepository playerPrefsRepository, ISubscriber<DoActionData> receiveActionData)
        {
            _gameManager = gameManager;
            _scoreManager = scoreManager;
            _gameSettings = gameSettings;
            _audioService = audioService;
            _socialService = socialService;
            _playerPrefsRepository = playerPrefsRepository;
            _receiveActionData = receiveActionData;

            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            // 初期処理
            gameInfoView.SetTextRemainTime(30, false);
            gameInfoView.SetTextChocoScore(0);
            gameResultView.SetTextResultChoco(0);

            // 購読開始
            // ゲーム状態
            _gameManager
                .State
                .Subscribe(state =>
                {
                    switch (state)
                    {
                        case GameState.Ready:
                            var bgmPitch = _gameSettings.IsExpBoostOption ? 1.2f : 1.0f;
                            _audioService.PlayBGM(GameAudioType.BgmGamePlay, bgmPitch);
                            // 初期表示
                            gameInfoView.SetTextStartMessage("");
                            gameInfoView.SetActiveChocoScoreArea(false);
                            gameInfoView.SetActiveRemainTimeArea(false);
                            gameResultView.gameObject.SetActive(false);
                            // カウント開始
                            gameInfoView.ShowStartMessageAsync(
                                _audioService.PlayOneShot,
                                () => _gameManager.SetReserveState(GameState.Play),
                                _cancellationTokenSource.Token);
                            break;
                        case GameState.Play:
                            // ゲーム画面表示
                            gameInfoView.SetActiveStartMessageText(false);
                            gameInfoView.SetActiveChocoScoreArea(true);
                            gameInfoView.SetActiveRemainTimeArea(true);
                            break;
                        case GameState.Result:
                            _audioService.StopBGM();
                            _audioService.PlayOneShot(GameAudioType.SeWhistle);
                            // 結果表示
                            gameInfoView.gameObject.SetActive(false);
                            gameResultView.gameObject.SetActive(true);
                            // スコアとEXPを保存して表示
                            var score = _scoreManager.GetScore();
                            var exp = _scoreManager.GetAddExp();
                            var isBestScore = _playerPrefsRepository.GetBestChocoScore() < score;
                            if (isBestScore) _playerPrefsRepository.SaveBestChocoScore(score);
                            _playerPrefsRepository.SaveTotalChocoScore(score + _playerPrefsRepository.GetTotalChocoScore());
                            _playerPrefsRepository.SavePlayerExp(exp + _playerPrefsRepository.GetPlayerExp());
                            gameResultView.ShowResult(
                                score,
                                exp,
                                () => _audioService.PlayBGM(GameAudioType.BgmGameResult),
                                () =>
                                {
                                    _audioService.PlayOneShot(GameAudioType.SeDecide);
                                    _gameManager.OnNextScene();
                                },
                                () =>
                                {
                                    var tweetText = $"チョコを{score}個集めました！\n#怪盗チョコレート\n◆iOS\nhttps://apple.co/3lsao7e\n◆Android\nhttps://play.google.com/store/apps/details?id=com.molegoro.chocolate";
                                    _socialService.TweetWithScreenShotAsync(tweetText, null);
                                },
                                _audioService.PlayOneShot, isBestScore);
                            break;
                    }
                }).AddTo(this);

            // 残り時間
            _gameManager
                .RemainTime
                .Subscribe(remainTime =>
                {
                    gameInfoView.SetTextRemainTime(remainTime, true);
                }).AddTo(this);

            // チョコスコア
            _scoreManager
                .ChocoScore
                .Subscribe(score =>
                {
                    gameInfoView.SetTextChocoScoreAsync(score, () => _audioService.PlayOneShot(GameAudioType.SeOk), _cancellationTokenSource.Token);
                }).AddTo(this);

            // プレイヤーアクション通知
            if (_gameSettings.IsDrawEffect)
            {
                _receiveActionData.Subscribe((actionType) =>
                {
                    gameEffectView.ShowComicLineEffect(_cancellationTokenSource.Token);
                }).AddTo(this);
            }
        }
    }
}
