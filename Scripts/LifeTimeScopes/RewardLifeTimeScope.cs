using Chocolate.UIs.Presenter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Chocolate.LifeTimeScopes
{
    public class RewardLifeTimeScope : LifetimeScope
    {
        [SerializeField] private RewardPresenter rewardPresenter;
        protected override void Configure(IContainerBuilder builder)
        {
            // Components
            builder.RegisterComponent(rewardPresenter);
        }
    }
}
