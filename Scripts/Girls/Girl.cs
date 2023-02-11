using System.Threading;
using Chocolate.Audio;
using Chocolate.Common;
using Chocolate.Const;
using Chocolate.Roads;
using Chocolate.Services;
using Chocolate.Settings;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Chocolate.Girls
{
    public class Girl
    {
        private readonly IAudioService _audioService;
        private readonly GirlBehaviour _girlBehaviour;
        private readonly GameSettings _gameSettings;
        private readonly StateMachine<Girl> _stateMachine;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        public Girl(GameObject girlPrefab, IAudioService audioService, GameSettings gameSettings)
        {
            _audioService = audioService;
            _gameSettings = gameSettings;

            var girlObj = Object.Instantiate(girlPrefab);
            _girlBehaviour = girlObj.GetComponent<GirlBehaviour>();

            // stateMachine初期化
            _stateMachine = new StateMachine<Girl>(this);
            _stateMachine.Add<StateWait>((int) GirlState.Wait);
            _stateMachine.Add<StateWalk>((int) GirlState.Walk);
            _stateMachine.Add<StateCheck>((int) GirlState.Check);
            _stateMachine.Add<StateAngry>((int) GirlState.Angry);
            _stateMachine.OnStart((int) GirlState.Wait);

            // キャンセルトークン発行
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }

        private GirlState _reserveState = GirlState.None;

        public bool IsCurrentState(GirlState state)
        {
            return _stateMachine.IsCurrentState((int)state);
        }

        public bool IsAngryMove()
        {
            return _girlBehaviour.IsAngryMove();
        }

        public void SetGirlId(int id)
        {
            _girlBehaviour.girlId = id;
        }

        // 歩行パス
        private RoadPassesBehaviour.RoadPass _roadPass;
        public void StartWalk(RoadPassesBehaviour.RoadPass roadPass)
        {
            _roadPass = roadPass;
        }

        // ----- 待機 -----
        private class StateWait : StateMachine<Girl>.StateBase
        {
            public override void OnStart()
            {
                // 待機中は非表示にしておく
                Owner._girlBehaviour.HideGirl();
                Owner._girlBehaviour.SetActiveGirlMarker(true);
            }

            public override void OnUpdate()
            {
                // 歩行パスが設定されたら状態遷移
                if (Owner._roadPass != null)
                {
                    Owner._stateMachine.ChangeState((int) GirlState.Walk);
                    // 歩行開始と同時に表示する
                    var bonusMultiply = Owner._gameSettings.IsExpBoostOption ? 5 : 1; // EXPボーナスの場合は5倍
                    var isBonus = Random.Range(0, 100) < 15 * bonusMultiply; // 一定確率でボーナスキャラにする
                    Owner._girlBehaviour.ShowGirl(Owner._roadPass, isBonus);
                    return;
                }
            }
            public override void OnEnd() { }
        }

        // ----- 歩行 -----
        private class StateWalk : StateMachine<Girl>.StateBase
        {
            public override void OnStart() { }

            public override void OnUpdate()
            {
                Owner._girlBehaviour.UpdateGirlMarker();
                // アクションを受けたらチェックする
                if (Owner._girlBehaviour.IsReceivePlayerAction())
                {
                    Owner._girlBehaviour.SetActiveGirlMarker(false);
                    StateMachine.ChangeState((int) GirlState.Check);
                    return;
                }
                // 目的地に到着したら待機中に戻す
                if (Owner._girlBehaviour.IsArriveTarget())
                {
                    Owner._roadPass = null;
                    Owner._stateMachine.ChangeState((int) GirlState.Wait);
                    return;
                }
                // 歩行させる
                Owner._girlBehaviour.UpdateMoveTarget();
            }
            public override void OnEnd() { }
        }

        // ----- チェック -----
        private class StateCheck : StateMachine<Girl>.StateBase
        {
            public override void OnStart()
            {
                // プレイヤーの方を向かせる
                var player = GameObject.FindWithTag(GameConst.TagNamePlayer); // めんどくさいのでFindしてしまう
                Owner._girlBehaviour.LookAtPlayer(player);

                // チェック開始
                CheckResultAsync(Owner._cancellationTokenSource.Token);
            }

            public override void OnUpdate()
            {
                if (Owner._reserveState != GirlState.None)
                {
                    Owner._stateMachine.ChangeState((int) Owner._reserveState);
                    Owner._reserveState = GirlState.None;
                    return;
                }
            }
            public override void OnEnd() { }

            private async void CheckResultAsync(CancellationToken token)
            {
                Owner._reserveState = GirlState.None;
                await UniTask.Delay(700, cancellationToken: token);
                Owner._girlBehaviour.StartTakeAnimation();
                await UniTask.Delay(500, cancellationToken: token);
                Owner._reserveState = Owner._girlBehaviour.GetCheckResultGirlState();
                Owner._girlBehaviour.ResetReceivePlayerAction();
            }
        }

        // ----- 怒り -----
        private class StateAngry : StateMachine<Girl>.StateBase
        {
            private GameObject _player;
            private bool _isStartMove;
            private bool _isEndMove;
            public override void OnStart()
            {
                Owner._audioService.PlayOneShot(GameAudioType.SeAngry);
                _player = GameObject.FindWithTag(GameConst.TagNamePlayer); // めんどくさいのでFindしてしまう
                _isStartMove = false;
                _isEndMove = false;
                Owner._girlBehaviour.StartAngry();
                AngryMoveAsync(Owner._cancellationTokenSource.Token);
            }
            public override void OnUpdate()
            {
                if (!_isStartMove) return;

                // 移動終了するか謝られたら怒りを鎮める
                var isDownAngry = Owner._girlBehaviour.isForceDownAngry;
                if (_isEndMove || isDownAngry)
                {
                    Owner._girlBehaviour.isForceDownAngry = false;
                    Owner._girlBehaviour.EndAngry();
                    Owner._girlBehaviour.ResetReceivePlayerAction(isDownAngry);
                    StateMachine.ChangeState((int) GirlState.Walk);
                    return;
                }
                Owner._girlBehaviour.UpdateAngryMove(_player);
            }
            public override void OnEnd() { }

            private async void AngryMoveAsync(CancellationToken token)
            {
                await UniTask.Delay(1000, cancellationToken: token);
                _isStartMove = true;
                await UniTask.Delay(3500, cancellationToken: token);
                _isEndMove = true;
            }
        }
    }
}
