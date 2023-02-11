using Chocolate.Audio;
using Chocolate.Common;
using Chocolate.Const;
using Chocolate.Data;
using Chocolate.Managers;
using Chocolate.Players;
using Chocolate.Players.Action;
using Chocolate.Services;
using Chocolate.Settings;
using Chocolate.UIs.View;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace Chocolate.UIs.Presenter
{
    public class TitlePresenter : MonoBehaviour
    {
        [SerializeField] private TitleTopView titleTopView;
        [SerializeField] private TitleTuneUpView titleTuneUpView;
        [SerializeField] private TitleTimelineView titleTimelineView;
        [SerializeField] private TitleHelpView titleHelpView;
        [SerializeField] private TitleBoostView titleBoostView;

        private TitleManager _titleManager;
        private IAudioService _audioService;
        private IStoreReviewService _storeReviewService;
        private ITransitionService _transitionService;
        private GameSettings _gameSettings;
        private PlayerPrefsRepository _playerPrefsRepository;
        private TitlePlayerBehaviour _titlePlayerBehaviour;

        private PlayerStatusModel _playerStatusModel;

        [Inject]
        public void Construct(TitleManager titleManager, IAudioService audioService, IStoreReviewService storeReviewService, ITransitionService transitionService, GameSettings gameSettings, PlayerPrefsRepository playerPrefsRepository, TitlePlayerBehaviour titlePlayerBehaviour)
        {
            _titleManager = titleManager;
            _audioService = audioService;
            _storeReviewService = storeReviewService;
            _transitionService = transitionService;
            _gameSettings = gameSettings;
            _playerPrefsRepository = playerPrefsRepository;
            _titlePlayerBehaviour = titlePlayerBehaviour;

            // PlayerPrefsからの読み込み
            LoadIsDrawEffectByPlayerPrefs();
            LoadPlayerStatusByPlayerPrefs();
            // ステータスの設定
            _playerStatusModel = new PlayerStatusModel();
            _playerStatusModel.InitializePlayerStatus(_gameSettings.PlayerStatusCharm, _gameSettings.PlayerStatusTech, _gameSettings.PlayerStatusSpeed, _gameSettings.PlayerExp);
        }

        private void LoadIsDrawEffectByPlayerPrefs()
        {
            var isDrawEffect = _playerPrefsRepository.GetIsDrawEffect();
            _gameSettings.IsDrawEffect = isDrawEffect;
        }

        private void LoadPlayerStatusByPlayerPrefs()
        {
            // ステータス
            var savePlayerStatus = _playerPrefsRepository.GetPlayerStatus();
            if (savePlayerStatus.CharmParameter > 0
                || savePlayerStatus.TechParameter > 0
                || savePlayerStatus.SpeedParameter > 0)
            {
                _gameSettings.PlayerStatusCharm = savePlayerStatus.CharmParameter;
                _gameSettings.PlayerStatusTech = savePlayerStatus.TechParameter;
                _gameSettings.PlayerStatusSpeed = savePlayerStatus.SpeedParameter;
            }
            // EXP
            var savePlayerExp = _playerPrefsRepository.GetPlayerExp();
            if (savePlayerExp > 0)
            {
                _gameSettings.PlayerExp = savePlayerExp;
            }
        }

        private void Awake()
        {
            // EXPブーストオプションは初めにリセットしておく
            _gameSettings.IsExpBoostOption = false;

            // TuneUp画面に直接遷移する場合
            if (_gameSettings.IsTransitionTuneUp)
            {
                _audioService.PlayBGM(GameAudioType.BgmTitleTop);
                titleTopView.gameObject.SetActive(false);
                titleHelpView.gameObject.SetActive(false);
                titleBoostView.gameObject.SetActive(false);
                titleTuneUpView.gameObject.SetActive(true);
                titleTopView.SetActiveSpecialThanksText(true);
                OnInitializeUIs();
                return;
            }

            // Title画面に遷移
            PlayTitleBGMAsync();
            titleTopView.gameObject.SetActive(true);
            titleHelpView.gameObject.SetActive(false);
            titleBoostView.gameObject.SetActive(false);
            titleTuneUpView.gameObject.SetActive(false);
            // タイトルアニメーション再生
            titleTopView.SetActiveTitleAnimationObjects(true);
            titleTopView.SetActiveAllButtonArea(false);
            titleTimelineView.StartTitleTimeline(() =>
            {
                titleTopView.SetActiveTitleAnimationObjects(false);
                titleTopView.SetActiveAllButtonArea(true);
                // タイトルアニメーション開始
                titleTopView.StartTitleImageAnimation(_audioService.PlayOneShot, () =>
                {
                    // Rewardシーンから遷移してきて、まだ一度もストアレビューを表示していない場合
                    if (_transitionService.IsEqualPrevScene(GameConst.SceneNameReward) &&
                        !_playerPrefsRepository.GetIsEvenOnceStoreReview())
                    {
                        _playerPrefsRepository.SaveIsEvenOnceStoreReview(true);
                        _storeReviewService.ShowStoreReview();
                    }
                });
                OnInitializeUIs();
            });
        }

        private void Update()
        {
            if (titleTimelineView.IsPlayAnimation() && TouchUtil.IsScreenTouch(true))
            {
                titleTimelineView.SkipAnimation();
            }
        }

        private async void PlayTitleBGMAsync()
        {
            await UniTask.Delay(50); // Timeline遅延があるため若干遅らせる
            _audioService.PlayBGM(GameAudioType.BgmTitleTop);
        }

        private void OnInitializeUIs()
        {
            // STARTボタン押下でTuneUp画面を表示
            titleTopView.SetListenerStartButton(() =>
            {
                _audioService.PlayOneShot(GameAudioType.SeClick);
                titleTopView.gameObject.SetActive(false);
                titleTuneUpView.gameObject.SetActive(true);
                // 一番最初のみヘルプウィンドウを開く
                if (!_playerPrefsRepository.GetIsEvenOnceHelpWindow())
                {
                    _playerPrefsRepository.SaveIsEvenOnceHelpWindow(true);
                    titleHelpView.ShowHelpWindow(() =>
                    {
                        _audioService.PlayOneShot(GameAudioType.SeClick);
                    }, () =>
                    {
                        _audioService.PlayOneShot(GameAudioType.SeDecide);
                    });
                }
            });

            // MORE APPボタン
            titleTopView.SetListenerMoreAppButton(() =>
            {
#if UNITY_ANDROID
                Application.OpenURL("https://play.google.com/store/apps/developer?id=MOLEGORO");
#elif UNITY_IPHONE
                Application.OpenURL("https://apps.apple.com/jp/developer/masato-watanabe/id1523138920");
#else
                Application.OpenURL("https://elekibear.com/molegoro_app");
#endif
            });

            // BGMボタン
            titleTopView.SetListenerBgmButton(() =>
            {
                var isVolumeOff = _audioService.ChangeBgmVolumeOnOff();
                titleTopView.ChangeBgmOnOffSprite(isVolumeOff);
                _playerPrefsRepository.SaveIsBgmVolumeOff(isVolumeOff);
            });
            titleTopView.ChangeBgmOnOffSprite(_playerPrefsRepository.GetIsBgmVolumeOff());

            // SEボタン
            titleTopView.SetListenerSeButton(() =>
            {
                var isVolumeOff = _audioService.ChangeSeVolumeOnOff();
                titleTopView.ChangeSeOnOffSprite(isVolumeOff);
                _playerPrefsRepository.SaveIsSeVolumeOff(isVolumeOff);
            });
            titleTopView.ChangeSeOnOffSprite(_playerPrefsRepository.GetIsSeVolumeOff());

            // ハートボタン
            var isActiveHeartButton = _playerPrefsRepository.GetIsClearStatusMax();
            titleTopView.SetActiveHeartButton(isActiveHeartButton);
            titleTopView.SetListenerHeartButton(() =>
            {
                _audioService.PlayOneShot(GameAudioType.SeDecide);
                _transitionService.LoadScene(GameConst.SceneNameReward);
            });

            // BACKボタン押下でTitleTop画面を表示
            titleTuneUpView.SetListenerBackButton(() =>
            {
                _audioService.PlayOneShot(GameAudioType.SeClick);
                titleTopView.gameObject.SetActive(true);
                titleTuneUpView.gameObject.SetActive(false);
            });

            // HELPボタン
            titleTuneUpView.SetListenerHelpButton(() =>
            {
                _audioService.PlayOneShot(GameAudioType.SeClick);
                titleHelpView.ShowHelpWindow(() =>
                {
                    _audioService.PlayOneShot(GameAudioType.SeClick);
                }, () =>
                {
                    _audioService.PlayOneShot(GameAudioType.SeDecide);
                });
            });

            // GOボタン押下でGameScene遷移
            titleTuneUpView.SetActiveGoButton(true);
            titleTuneUpView.SetListenerGoButton(() =>
            {
                if (_gameSettings.IsExpBoostOption)
                {
                    _audioService.PlayOneShot(GameAudioType.SeClick);
                    titleBoostView.ShowBoostWindow();
                    return;
                }
                // ステータスMAXかつ未クリアの場合、ブーストする
                if (_playerStatusModel.IsAllMaxParameter() && !_playerPrefsRepository.GetIsClearStatusMax())
                {
                    _audioService.PlayOneShot(GameAudioType.SeOkBonus);
                    _gameSettings.IsExpBoostOption = true;
                }
                // 確率を計算して遷移
                _audioService.PlayOneShot(GameAudioType.SeDecide);
                SetSettingsCalculateSuccessPercent();
                _transitionService.LoadScene(GameConst.SceneNameGame);
            });

            // EXPブーストウィンドウ
            titleBoostView.SetListenerOkButton(() =>
            {
                // 必要EXPを引く
                var remainExp = _gameSettings.PlayerExp - NeedExpBoostValue;
                _gameSettings.PlayerExp = remainExp;
                _playerPrefsRepository.SavePlayerExp(remainExp);
                // 確率を計算して遷移
                _audioService.PlayOneShot(GameAudioType.SeDecide);
                _audioService.PlayOneShot(GameAudioType.SeOkBonus);
                SetSettingsCalculateSuccessPercent();
                _transitionService.LoadScene(GameConst.SceneNameGame);
            });
            titleBoostView.SetListenerCancelButton(() =>
            {
                _audioService.PlayOneShot(GameAudioType.SeClick);
                titleBoostView.gameObject.SetActive(false);
            });
            // EXPブーストオプション
            var isClear = _playerPrefsRepository.GetIsClearStatusMax();
            titleTuneUpView.SetActiveBoostOptionArea(isClear); // クリアしていたら表示する
            titleTuneUpView.SetIsDisabledBoostOptionToggle(IsDisabledBoostOptionToggle());
            titleTuneUpView.SetIsOnBoostOptionToggle(false);
            titleTuneUpView.SetListenerBoostOptionToggle((isOn) =>
            {
                _audioService.PlayOneShot(GameAudioType.SeClick);
                _gameSettings.IsExpBoostOption = isOn;
            });

            // 強化ボタン
            titleTuneUpView.SetActiveTuneButton(false); // 初期表示は非活性
            titleTuneUpView.SetListenerTuneButton(() =>
            {
                _audioService.PlayOneShot(GameAudioType.SeActionApproach);
                _titlePlayerBehaviour.StartTuneUpAnimation();
                // ステータス、確率を計算して設定
                SaveCalculatePlayerStatus();
                ResetPlayerStatus();
                SetSettingsCalculateSuccessPercent();
                UpdatePlayerStatusValue();
            });

            // エフェクト描画オプション
            titleTuneUpView.SetIsOnEffectOptionToggle(_gameSettings.IsDrawEffect);
            titleTuneUpView.SetListenerEffectOptionToggle((isOn) =>
            {
                _audioService.PlayOneShot(GameAudioType.SeClick);
                _gameSettings.IsDrawEffect = isOn;
                _playerPrefsRepository.SaveIsDrawEffect(isOn);
            });

            // リセットボタン
            titleTuneUpView.SetListenerStatusResetButton(() =>
            {
                _audioService.PlayOneShot(GameAudioType.SeClick);
                ResetPlayerStatus();
                UpdatePlayerStatusValue();
            });

            // スコア関連
            titleTuneUpView.SetTextTotalScore(_playerPrefsRepository.GetTotalChocoScore());
            titleTuneUpView.SetTextBestScore(_playerPrefsRepository.GetBestChocoScore());

            // プレイヤーステータス関連
            titleTuneUpView.SetListenerPlayerStatusCharmPlusButton(() => OnPushPlayerStatusPlusButton(PlayerStatusModel.PlayerStatusType.Charm));
            titleTuneUpView.SetListenerPlayerStatusTechPlusButton(() => OnPushPlayerStatusPlusButton(PlayerStatusModel.PlayerStatusType.Tech));
            titleTuneUpView.SetListenerPlayerStatusSpeedPlusButton(() => OnPushPlayerStatusPlusButton(PlayerStatusModel.PlayerStatusType.Speed));
            UpdatePlayerStatusValue();
        }

        private void SaveCalculatePlayerStatus()
        {
            // Settings、PlayerPrefsに保存する
            var remainExp = _playerStatusModel.GetRemainExp();
            _gameSettings.PlayerExp = remainExp;
            _playerPrefsRepository.SavePlayerExp(remainExp);
            var displayPlayerStatus = _playerStatusModel.GetDisplayPlayerStatus();
            _gameSettings.PlayerStatusCharm = displayPlayerStatus.CharmParameter;
            _gameSettings.PlayerStatusTech = displayPlayerStatus.TechParameter;
            _gameSettings.PlayerStatusSpeed = displayPlayerStatus.SpeedParameter;
            _playerPrefsRepository.SavePlayerStatus(displayPlayerStatus.CharmParameter, displayPlayerStatus.TechParameter, displayPlayerStatus.SpeedParameter);
        }

        private void SetSettingsCalculateSuccessPercent()
        {
            // 計算した確率を設定
            _gameSettings.ApproachActionInfo.successPercent = _playerStatusModel.CalculateActionSuccessPercent(
                _gameSettings.ApproachActionInfo.minSuccessPercent,
                _gameSettings.ApproachActionInfo.maxSuccessPercent,
                PlayerActionType.Approach);
            _gameSettings.PleaseActionInfo.successPercent = _playerStatusModel.CalculateActionSuccessPercent(
                _gameSettings.PleaseActionInfo.minSuccessPercent,
                _gameSettings.PleaseActionInfo.maxSuccessPercent,
                PlayerActionType.Please);
            _gameSettings.ThiefActionInfo.successPercent = _playerStatusModel.CalculateActionSuccessPercent(
                _gameSettings.ThiefActionInfo.minSuccessPercent,
                _gameSettings.ThiefActionInfo.maxSuccessPercent,
                PlayerActionType.Thief);
            _gameSettings.PlayerSpeed = _playerStatusModel.CalculatePlayerSpeed(_gameSettings.MinPlayerSpeed, _gameSettings.MaxPlayerSpeed);
            Debug.Log($"successPercent: {_gameSettings.ApproachActionInfo.successPercent} {_gameSettings.PleaseActionInfo.successPercent} {_gameSettings.ThiefActionInfo.successPercent} {_gameSettings.PlayerSpeed}");
        }

        private void OnPushPlayerStatusPlusButton(PlayerStatusModel.PlayerStatusType playerStatusType)
        {
            _audioService.PlayOneShot(GameAudioType.SeClick);
            // 値を加算
            var plusValue = _gameSettings.AddPlayerStatus;
            if (_playerStatusModel.IsCanAddPlayerStatus(plusValue, playerStatusType))
            {
                _playerStatusModel.AddPlayerStatus(plusValue, playerStatusType);
                // 強化ボタンを表示
                titleTuneUpView.SetActiveGoButton(false);
                titleTuneUpView.SetActiveTuneButton(true);
                // EXPブーストトグルはリセットする
                titleTuneUpView.SetIsOnBoostOptionToggle(false);
                titleTuneUpView.SetIsDisabledBoostOptionToggle(true);
                // 値を更新
                UpdatePlayerStatusValue();
            }
        }

        private void ResetPlayerStatus()
        {
            // Modelを再度初期化
            _playerStatusModel.InitializePlayerStatus(_gameSettings.PlayerStatusCharm, _gameSettings.PlayerStatusTech, _gameSettings.PlayerStatusSpeed, _gameSettings.PlayerExp);
            // GOボタン表示
            titleTuneUpView.SetActiveGoButton(true);
            titleTuneUpView.SetActiveTuneButton(false);
            // EXPブーストトグルを再設定
            titleTuneUpView.SetIsDisabledBoostOptionToggle(IsDisabledBoostOptionToggle());
        }

        private void UpdatePlayerStatusValue()
        {
            // ステータス
            var displayPlayerStatus = _playerStatusModel.GetDisplayPlayerStatus();
            titleTuneUpView.SetTextPlayerStatusExp(_playerStatusModel.GetRemainExp());
            titleTuneUpView.SetTextPlayerStatusCharm(displayPlayerStatus.CharmParameter, _playerStatusModel.IsMaxParameter(displayPlayerStatus.CharmParameter));
            titleTuneUpView.SetTextPlayerStatusTech(displayPlayerStatus.TechParameter, _playerStatusModel.IsMaxParameter(displayPlayerStatus.TechParameter));
            titleTuneUpView.SetTextPlayerStatusSpeed(displayPlayerStatus.SpeedParameter, _playerStatusModel.IsMaxParameter(displayPlayerStatus.SpeedParameter));
            // 成功確率とスピード
            var approachValue = _playerStatusModel.CalculateActionSuccessPercent(
                _gameSettings.ApproachActionInfo.minSuccessPercent,
                _gameSettings.ApproachActionInfo.maxSuccessPercent,
                PlayerActionType.Approach);
            titleTuneUpView.SetTextApproachPercent(approachValue, approachValue == _gameSettings.ApproachActionInfo.maxSuccessPercent);
            var pleaseValue = _playerStatusModel.CalculateActionSuccessPercent(
                _gameSettings.PleaseActionInfo.minSuccessPercent,
                _gameSettings.PleaseActionInfo.maxSuccessPercent,
                PlayerActionType.Please);
            titleTuneUpView.SetTextPleasePercent(pleaseValue, pleaseValue == _gameSettings.PleaseActionInfo.maxSuccessPercent);
            var thiefValue = _playerStatusModel.CalculateActionSuccessPercent(
                _gameSettings.ThiefActionInfo.minSuccessPercent,
                _gameSettings.ThiefActionInfo.maxSuccessPercent,
                PlayerActionType.Thief);
            titleTuneUpView.SetTextThiefPercent(thiefValue, thiefValue == _gameSettings.ThiefActionInfo.maxSuccessPercent);
            var speedValue =
                _playerStatusModel.CalculatePlayerSpeed(_gameSettings.MinPlayerSpeed, _gameSettings.MaxPlayerSpeed);
            titleTuneUpView.SetTextPlayerSpeed(speedValue, Mathf.FloorToInt(speedValue) == Mathf.FloorToInt(_gameSettings.MaxPlayerSpeed));
        }

        private readonly int NeedExpBoostValue = 500;
        private bool IsDisabledBoostOptionToggle()
        {
            // 500EXP以下の場合
            return _gameSettings.PlayerExp < NeedExpBoostValue;
        }
    }
}
