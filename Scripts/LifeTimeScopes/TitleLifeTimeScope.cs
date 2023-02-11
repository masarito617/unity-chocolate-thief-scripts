using Chocolate.EntryPoints;
using Chocolate.Managers;
using Chocolate.Players;
using Chocolate.UIs.Presenter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Chocolate.LifeTimeScopes
{
    public class TitleLifeTimeScope : LifetimeScope
    {
        [SerializeField] private TitlePresenter titlePresenter;
        [SerializeField] private TitlePlayerBehaviour titlePlayerBehaviour;

        protected override void Configure(IContainerBuilder builder)
        {
            // Singletons
            builder.Register<TitleManager>(Lifetime.Singleton);

            // Components
            builder.RegisterComponent(titlePresenter);
            builder.RegisterComponent(titlePlayerBehaviour);

            // EntryPoints
            builder.RegisterEntryPoint<TitleEntryPoint>();
        }
    }
}
