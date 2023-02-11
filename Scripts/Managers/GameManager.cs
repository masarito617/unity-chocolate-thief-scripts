using System;
using System.Threading;
using Chocolate.Common;
using Chocolate.Const;
using Chocolate.Data;
using Chocolate.Players;
using Chocolate.Services;
using Chocolate.Settings;
using UniRx;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Chocolate.Managers
{
    public class GameManager : IDisposable
    {
        private readonly PlayerManager _playerManager;
        private readonly GirlManager _girlManager;
        private readonly ScoreManager _scoreManager;
        private readonly ITransitionService _transitionService;
        private readonly PlayerPrefsRepository _playerPrefsRepository;
        private readonly GameSettings _gameSettings;

        /// <summary>
        /// 残り時間
        /// </summary>
        private ReactiveProperty<float> _remainTime = new(MaxRemainTimeValue);
        public IReactiveProperty<float> RemainTime => _remainTime;
        private static readonly int MaxRemainTimeValue = 30;

        /// <summary>
        /// ゲーム状態
        /// </summary>
        private readonly ReactiveProperty<GameState> _state = new(GameState.Play);
        public IReadOnlyReactiveProperty<GameState> State => _state;
        private readonly StateMachine<GameManager> _stateMachine;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public GameManager(PlayerManager playerManager, GirlManager girlManager, ScoreManager scoreManager, ITransitionService transitionService, PlayerPrefsRepository playerPrefsRepository, GameSettings gameSettings)
        {
            _playerManager = playerManager;
            _girlManager = girlManager;
            _scoreManager = scoreManager;
            _transitionService = transitionService;
            _playerPrefsRepository = playerPrefsRepository;
            _gameSettings = gameSettings;

            // ステートマシン設定
            _stateMachine = new StateMachine<GameManager>(this);
            _stateMachine.SetChangeStateEvent((stateId) =>
            {
                // ステート変更時にReactivePropertyにも反映
                _state.Value = (GameState) Enum.ToObject(typeof(GameState), stateId);
            });
            _stateMachine.Add<StateReady>((int) GameState.Ready);
            _stateMachine.Add<StatePlay>((int) GameState.Play);
            _stateMachine.Add<StateResult>((int) GameState.Result);

            // キャンセルトークン発行
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void OnStart()
        {
            // 初期化
            _scoreManager.OnInitialize();

            // ステートマシン開始
            _stateMachine.OnStart((int) GameState.Ready);
        }

        public void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }

        public void Dispose()
        {
            _remainTime.Dispose();
        }

        // 予約シーン
        private GameState _reserveState = GameState.None;
        public void SetReserveState(GameState state)
        {
            _reserveState = state;
        }

        public void OnNextScene()
        {
            // 一度でもゲームをプレイした場合はチューンアップに直接遷移
            _gameSettings.IsTransitionTuneUp = true;

            // 全てのパラメータがMAXの状態でクリアした場合、エンディング画面へ遷移する
            var statusModel = new PlayerStatusModel();
            statusModel.InitializePlayerStatus(_gameSettings.PlayerStatusCharm, _gameSettings.PlayerStatusTech, _gameSettings.PlayerStatusSpeed, _gameSettings.PlayerExp);
            if (statusModel.IsAllMaxParameter() && !_playerPrefsRepository.GetIsClearStatusMax())
            {
                _playerPrefsRepository.SaveIsClearStatusMax(true);
                _transitionService.LoadScene(GameConst.SceneNameReward);
                return;
            }

            // 累計チョコが一定以上 かつ 前回の広告再生から一定回数以上プレイした場合 かつ 一定確率 の場合、動画広告を再生
            var totalChoco = _playerPrefsRepository.GetTotalChocoScore();
            _gameSettings.LastAdmobPlayCount += 1;
            var lastAdmobPlayCount = _gameSettings.LastAdmobPlayCount;
            if (totalChoco >= 100 & lastAdmobPlayCount >= 3 && Random.Range(0, 100) < 25)
            {
                _gameSettings.LastAdmobPlayCount = 0;
                _transitionService.LoadScene(GameConst.SceneNameInterstitial);
                return;
            }

            // それ以外はタイトルに遷移
            _transitionService.LoadScene(GameConst.SceneNameTitle);
        }

        // ----- 準備 -----
        private class StateReady : StateMachine<GameManager>.StateBase
        {
            public override void OnStart()
            {
                // プレイヤー、女の子の生成
                Owner._playerManager.CreatePlayer();
                Owner._girlManager.CreateGirls();
            }

            public override void OnUpdate()
            {
                // Presenter側で設定されるまで待つ
                if (Owner._reserveState != GameState.None)
                {
                    Owner._stateMachine.ChangeState((int) Owner._reserveState);
                    Owner._reserveState = GameState.None;
                    return;
                }

                // プレイヤーのみ更新
                Owner._playerManager.UpdatePlayer();
            }
            public override void OnEnd() { }
        }

        // ----- プレイ -----
        private class StatePlay : StateMachine<GameManager>.StateBase
        {
            private float _totalProgressTime = 0.0f;
            public override void OnStart()
            {
                // 移動開始
                Owner._playerManager.StartMove();
                _totalProgressTime = 0.0f;
            }

            public override void OnUpdate()
            {
                // 残り時間の計算
                _totalProgressTime += Time.deltaTime;
                Owner.RemainTime.Value = MaxRemainTimeValue - _totalProgressTime;
                if (Owner.RemainTime.Value <= 0.0f || Owner._playerManager.IsDeadPlayer())
                {
                    Owner._playerManager.SetActiveGameCtrlView(false);
                    StateMachine.ChangeState((int) GameState.Result);
                    return;
                }

                // プレイヤー、女の子を更新
                Owner._playerManager.UpdatePlayer();
                Owner._girlManager.UpdateGirls(GetMaxActiveCount(Owner.RemainTime.Value));
            }
            public override void OnEnd() { }

            // 女の子の最大出現数
            // 本来はパラメータ化するべきだが時間がないので直指定
            private int GetMaxActiveCount(float remainTime)
            {
                // 残り秒数によって出現数を変える
                if (remainTime > 20.0f)
                {
                    return 3;
                }
                if (remainTime > 10.0f)
                {
                    return 4;
                }
                return 5;
            }
        }

        // ----- 結果 -----
        private class StateResult : StateMachine<GameManager>.StateBase
        {
            public override void OnStart() { }
            public override void OnUpdate() { }
            public override void OnEnd() { }
        }
    }
}
