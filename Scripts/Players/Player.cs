using System.Threading;
using Chocolate.Audio;
using Chocolate.Common;
using Chocolate.Data.Event;
using Chocolate.Players.Action;
using Chocolate.Players.Input;
using Chocolate.Services;
using Chocolate.Settings;
using Cysharp.Threading.Tasks;
using MessagePipe;
using UnityEngine;

namespace Chocolate.Players
{
    public class Player
    {
        private readonly PlayerBehaviour _playerBehaviour;
        private readonly IInputProvider _inputProvider;
        private readonly IPublisher<ScoreData> _onSendScoreEvent;
        private readonly IPublisher<DoActionData> _doPlayerActionEvent;
        private readonly IAudioService _audioService;
        private readonly GameSettings _gameSettings;
        private readonly StateMachine<Player> _stateMachine;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Player(GameObject playerPrefab, IInputProvider inputProvider, IPublisher<ScoreData> onSendScoreEvent, IPublisher<DoActionData> doPlayerActionEvent, IAudioService audioService, GameSettings gameSettings)
        {
            var playerObj = Object.Instantiate(playerPrefab);
            _playerBehaviour = playerObj.GetComponent<PlayerBehaviour>();
            _inputProvider = inputProvider;
            _onSendScoreEvent = onSendScoreEvent;
            _doPlayerActionEvent = doPlayerActionEvent;
            _audioService = audioService;
            _gameSettings = gameSettings;

            // stateMachine初期化
            _stateMachine = new StateMachine<Player>(this);
            _stateMachine.Add<StateWait>((int) PlayerState.Wait);
            _stateMachine.Add<StateMove>((int) PlayerState.Move);
            _stateMachine.Add<StateApproach>((int) PlayerState.Approach);
            _stateMachine.Add<StatePlease>((int) PlayerState.Please);
            _stateMachine.Add<StateThief>((int) PlayerState.Thief);
            _stateMachine.Add<StateDead>((int) PlayerState.Dead);
            _stateMachine.OnStart((int) PlayerState.Wait);

            // InputProvider初期化
            _inputProvider.OnInitialize(
                _gameSettings.ApproachActionInfo.successPercent,
                _gameSettings.PleaseActionInfo.successPercent,
                _gameSettings.ThiefActionInfo.successPercent);

            // キャンセルトークン発行
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }

        private bool _isDead = false;
        public bool IsDead()
        {
            return _isDead;
        }

        private PlayerState _reserveState = PlayerState.None;
        public void StartMove()
        {
            _reserveState = PlayerState.Move;
        }

        private async UniTask GetChocoTask(int getChocoCount, CancellationToken token)
        {
            await UniTask.Delay(200, cancellationToken: token);
            _onSendScoreEvent.Publish(new ScoreData()
            {
                GetChocoCount = getChocoCount
            });
            _reserveState = PlayerState.Move;
        }

        public void SetActiveGameCtrlView(bool isActive)
        {
            _inputProvider.SetActiveView(isActive);
            // 動いていたら停止させる
            if (!isActive) _playerBehaviour.UpdateMoveVelocity(Vector2.zero, _gameSettings.PlayerSpeed);
        }

        private int GetBaseResultChocoCount(PlayerActionType actionType)
        {
            // 本来は外出しするべきだが面倒なのでこのまま
            var count = 0;
            switch (actionType)
            {
                // アプローチ: 3-5
                case PlayerActionType.Approach:
                    var countManyArray = new int[4] {3, 3, 4, 5};
                    count = countManyArray[Random.Range(0, countManyArray.Length)];
                    break;
                // 土下座、強奪: 1-3
                case PlayerActionType.Please:
                case PlayerActionType.Thief:
                    var countArray = new int[4] {1, 1, 2, 3};
                    count = countArray[Random.Range(0, countArray.Length)];
                    break;
            }
            return count;
        }

        // ----- 待機 -----
        private class StateWait : StateMachine<Player>.StateBase
        {
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // 怒っている女の子に衝突したら死亡
                if (Owner._playerBehaviour.IsTriggerAngryMoveGirl())
                {
                    Owner._isDead = true;
                    StateMachine.ChangeState((int) PlayerState.Dead);
                    return;
                }
                if (Owner._reserveState != PlayerState.None)
                {
                    Owner._stateMachine.ChangeState((int) Owner._reserveState);
                    Owner._reserveState = PlayerState.None;
                    return;
                }
            }
            public override void OnEnd() { }
        }

        // ----- 移動 -----
        private class StateMove : StateMachine<Player>.StateBase
        {
            public override void OnStart() { }

            public override void OnUpdate()
            {
                // 怒っている女の子に衝突したら死亡
                if (Owner._playerBehaviour.IsTriggerAngryMoveGirl())
                {
                    Owner._isDead = true;
                    StateMachine.ChangeState((int) PlayerState.Dead);
                    return;
                }
                // 入力アクション取得
                var inputRunAction = Owner._inputProvider.GetPlayerRunAction();
                // 怒り中の女の子に対してのみ土下座できるようにする
                if (inputRunAction == PlayerActionType.Please && Owner._playerBehaviour.CheckPleaseActionTriggerAngryGirls())
                {
                    StateMachine.ChangeState((int) PlayerState.Please);
                    Owner._playerBehaviour.UpdateMoveVelocity(Vector2.zero, Owner._gameSettings.PlayerSpeed);
                    Owner._playerBehaviour.SetActionTrigger(PlayerActionType.Please);
                    return;
                }
                // アクションの実行
                var runAction = Owner._playerBehaviour.RunAction(inputRunAction, Owner._gameSettings);
                if (runAction != PlayerActionType.None)
                {
                    // アクションを実行した場合、ステートを変更する
                    Owner._doPlayerActionEvent.Publish(new DoActionData() {DoPlayerActionType = runAction});
                    switch (runAction)
                    {
                        case PlayerActionType.Approach:
                            StateMachine.ChangeState((int) PlayerState.Approach);
                            break;
                        case PlayerActionType.Please:
                            StateMachine.ChangeState((int) PlayerState.Please);
                            break;
                        case PlayerActionType.Thief:
                            StateMachine.ChangeState((int) PlayerState.Thief);
                            break;
                    }
                    // 動いていたら停止させる
                    Owner._playerBehaviour.UpdateMoveVelocity(Vector2.zero, Owner._gameSettings.PlayerSpeed);
                    // アニメーション開始
                    Owner._playerBehaviour.SetActionTrigger(runAction);
                    return;
                }

                // 入力を検知して移動
                var moveVelocity = Owner._inputProvider.GetMoveVelocity();
                Owner._playerBehaviour.UpdateMoveVelocity(moveVelocity, Owner._gameSettings.PlayerSpeed);
            }
            public override void OnEnd() { }
        }

        // ----- アプローチ -----
        private class StateApproach : StateMachine<Player>.StateBase
        {
            public override void OnStart()
            {
                Owner._audioService.PlayOneShot(GameAudioType.SeActionApproach);
                CheckGetChocoAsync(Owner._cancellationTokenSource.Token);
            }

            public override void OnUpdate()
            {
                if (Owner._reserveState != PlayerState.None)
                {
                    StateMachine.ChangeState((int) Owner._reserveState);
                    Owner._reserveState = PlayerState.None;
                }
            }
            public override void OnEnd() { }

            private async void CheckGetChocoAsync(CancellationToken token)
            {
                Owner._reserveState = PlayerState.None;
                await UniTask.Delay(800, cancellationToken: token);
                if (Owner._playerBehaviour.IsSuccessGetChocolateAndReset())
                {
                    var chocoCount = Owner.GetBaseResultChocoCount(PlayerActionType.Approach)
                                     * Owner._playerBehaviour.GetSuccessChocoCountScale();
                    if (Owner._playerBehaviour.IsContainTriggerBonusGirl()) Owner._audioService.PlayOneShot(GameAudioType.SeOkBonus);
                    Owner._playerBehaviour.StartGetChocoAnimation();
                    await Owner.GetChocoTask(chocoCount , token);
                }
                else
                {
                    Owner._audioService.PlayOneShot(GameAudioType.SeNg);
                    Owner._reserveState = PlayerState.Move;
                }
            }
        }

        // ----- 土下座 -----
        private class StatePlease : StateMachine<Player>.StateBase
        {
            public override void OnStart()
            {
                Owner._audioService.PlayOneShot(GameAudioType.SeActionPlease);
                CheckGetChocoAsync(Owner._cancellationTokenSource.Token);
            }

            public override void OnUpdate()
            {
                if (Owner._reserveState != PlayerState.None)
                {
                    StateMachine.ChangeState((int) Owner._reserveState);
                    Owner._reserveState = PlayerState.None;
                }
            }
            public override void OnEnd() { }

            private async void CheckGetChocoAsync(CancellationToken token)
            {
                Owner._reserveState = PlayerState.None;
                await UniTask.Delay(700, cancellationToken: token);
                if (Owner._playerBehaviour.IsSuccessGetChocolateAndReset())
                {
                    var chocoCount = Owner.GetBaseResultChocoCount(PlayerActionType.Please)
                                     * Owner._playerBehaviour.GetSuccessChocoCountScale();
                    if (Owner._playerBehaviour.IsContainTriggerBonusGirl()) Owner._audioService.PlayOneShot(GameAudioType.SeOkBonus);
                    Owner._playerBehaviour.StartGetChocoAnimation();
                    await Owner.GetChocoTask(chocoCount, token);
                }
                else
                {
                    Owner._audioService.PlayOneShot(GameAudioType.SeNg);
                    Owner._reserveState = PlayerState.Move;
                }
            }
        }

        // ----- 強奪 -----
        private class StateThief : StateMachine<Player>.StateBase
        {
            public override void OnStart()
            {
                Owner._audioService.PlayOneShot(GameAudioType.SeActionThief);
                CheckGetChocoAsync(Owner._cancellationTokenSource.Token);
            }

            public override void OnUpdate()
            {
                if (Owner._reserveState != PlayerState.None)
                {
                    StateMachine.ChangeState((int) Owner._reserveState);
                    Owner._reserveState = PlayerState.None;
                }
            }
            public override void OnEnd() { }

            private async void CheckGetChocoAsync(CancellationToken token)
            {
                Owner._reserveState = PlayerState.None;

                // スライド
                await Owner._playerBehaviour.SlideMoveTask();

                await UniTask.Delay(400, cancellationToken: token);
                if (Owner._playerBehaviour.IsSuccessGetChocolateAndReset())
                {
                    var chocoCount = Owner.GetBaseResultChocoCount(PlayerActionType.Thief)
                                     * Owner._playerBehaviour.GetSuccessChocoCountScale();
                    if (Owner._playerBehaviour.IsContainTriggerBonusGirl()) Owner._audioService.PlayOneShot(GameAudioType.SeOkBonus);
                    Owner._playerBehaviour.StartGetChocoAnimation();
                    await Owner.GetChocoTask(chocoCount, token);
                }
                else
                {
                    Owner._audioService.PlayOneShot(GameAudioType.SeNg);
                    Owner._reserveState = PlayerState.Move;
                }
            }
        }

        // ----- 死亡 -----
        private class StateDead : StateMachine<Player>.StateBase
        {
            public override void OnStart()
            {
                Owner._audioService.PlayOneShot(GameAudioType.SeDead);
                Owner._playerBehaviour.StartDeadAnimation();
            }

            public override void OnUpdate() { }
            public override void OnEnd() { }
        }
    }
}
