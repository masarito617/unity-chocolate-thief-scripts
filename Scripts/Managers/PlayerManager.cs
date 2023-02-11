using Chocolate.Players;
using VContainer;

namespace Chocolate.Managers
{
    public class PlayerManager
    {
        private readonly PlayerFactory _playerFactory;

        private Player _player;

        [Inject]
        public PlayerManager(PlayerFactory playerFactory)
        {
            _playerFactory = playerFactory;
        }

        /// <summary>
        /// プレイヤー初期化
        /// </summary>
        public void CreatePlayer()
        {
            _player = _playerFactory.CreatePlayer();
        }

        /// <summary>
        /// 歩行開始
        /// </summary>
        public void StartMove()
        {
            _player.StartMove();
        }

        /// <summary>
        /// プレイヤー更新
        /// </summary>
        public void UpdatePlayer()
        {
            _player.OnUpdate();
        }

        /// <summary>
        /// 操作Viewの有効/無効切替
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActiveGameCtrlView(bool isActive)
        {
            _player.SetActiveGameCtrlView(isActive);
        }

        /// <summary>
        /// 死んでいる・・・？
        /// </summary>
        public bool IsDeadPlayer()
        {
            return _player.IsDead();
        }
    }
}
