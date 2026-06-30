using Code.GameApp.Boot;
using Code.Infrastructure.Services.Analytic;
using Code.Infrastructure.Services.SceneLoad;
using VContainer;
using VContainer.Unity;

namespace Code.GameApp.Installers.VContainer
{
   /// <summary>
   /// Composition root for the Bootstrap scene. Wires the handful of services the
   /// boot needs, then <see cref="BootInitializeSystem"/> loads the first level.
   /// Each gameplay scene carries its own self-contained scope, so global services
   /// are re-created per scene (state survives across scenes via the save file).
   /// </summary>
   public sealed class BootLifetimeScope : LifetimeScope
   {
      protected override void Configure(IContainerBuilder builder)
      {
         builder.Register<SceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();

         builder.UseEntryPoints(entries =>
         {
            entries.Add<AnalyticService>();        // IAnalyticService + IInitializable/ITickable/IDisposable
            entries.Add<BootInitializeSystem>();   // loads "1. Level1"
         });
      }
   }
}
