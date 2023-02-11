using Chocolate.Managers;
using VContainer;
using VContainer.Unity;

namespace Chocolate.EntryPoints
{
    public class GameEntryPoint : IStartable, ITickable
    {
        private readonly GameManager _gameManager;

        [Inject]
        public GameEntryPoint(GameManager gameManager)
        {
            _gameManager = gameManager;
        }

        void IStartable.Start()
        {
            _gameManager.OnStart();
        }

        void ITickable.Tick()
        {
            _gameManager.OnUpdate();
        }
    }
}
