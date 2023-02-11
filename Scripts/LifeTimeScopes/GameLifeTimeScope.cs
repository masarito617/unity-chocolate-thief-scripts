using Chocolate.Data.Event;
using Chocolate.EntryPoints;
using Chocolate.Girls;
using Chocolate.Managers;
using Chocolate.Players;
using Chocolate.Players.Input;
using Chocolate.Players.Input.Impl;
using Chocolate.Roads;
using Chocolate.Settings;
using Chocolate.UIs.Presenter;
using Chocolate.UIs.View;
using MessagePipe;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Chocolate.LifeTimeScopes
{
    public class GameLifeTimeScope : LifetimeScope
    {
        [SerializeField] private GamePresenter gamePresenter;
        [SerializeField] private GameCtrlView gameCtrlView;
        [SerializeField] private RoadPassesBehaviour roadPassesBehaviour;
        [SerializeField] private GameSettings gameSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            // Singleton Classes
            builder.Register<GameManager>(Lifetime.Singleton);
            builder.Register<PlayerManager>(Lifetime.Singleton);
            builder.Register<GirlManager>(Lifetime.Singleton);
            builder.Register<ScoreManager>(Lifetime.Singleton);
            builder.Register<PlayerFactory>(Lifetime.Singleton);
            builder.Register<GirlFactory>(Lifetime.Singleton);

            // モバイルかどうかで切り替える
            if (gameSettings.IsMobilePlatform)
            {
                builder.Register<IInputProvider, InputTouchProvider>(Lifetime.Singleton);
            }
            else
            {
                builder.Register<IInputProvider, InputKeyProvider>(Lifetime.Singleton);
            }

            // Components
            builder.RegisterComponent(gamePresenter);
            builder.RegisterComponent(gameCtrlView);
            builder.RegisterComponent(roadPassesBehaviour);

            // MessagePipes
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<ScoreData>(options);
            builder.RegisterMessageBroker<DoActionData>(options);

            // EntryPoints
            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}
