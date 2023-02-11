using Chocolate.EntryPoints;
using VContainer;
using VContainer.Unity;

namespace Chocolate.LifeTimeScopes
{
    public class InterstitialLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // エントリーポイント登録
            builder.RegisterEntryPoint<InterstitialEntryPoint>();
        }
    }
}