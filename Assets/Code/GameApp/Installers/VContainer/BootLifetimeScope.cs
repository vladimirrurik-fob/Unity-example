using Code.GameApp.Boot;
using Code.GameApp.States;
using Code.Infrastructure.Services.Analytic;
using Code.Infrastructure.Services.Curtain;
using Code.Infrastructure.Services.SceneLoad;
using Code.Infrastructure.Services.StateMachine;
using Code.Infrastructure.Services.StateMachine.Factory;
using VContainer;
using VContainer.Unity;

namespace Code.GameApp.Installers.VContainer
{
   /// <summary>
   /// Composition root for the Bootstrap scene. Wires the global state machine
   /// (Zenject-free, on VContainer) and its states plus the services they need.
   /// <see cref="BootInitializeSystem"/> enters <see cref="BootstrapState"/>, which
   /// warms up, logs, and transitions through <see cref="LoadLevelState"/> into the
   /// gameplay scene.
   /// </summary>
   public sealed class BootLifetimeScope : LifetimeScope
   {
      protected override void Configure(IContainerBuilder builder)
      {
         // --- services the states depend on ---
         builder.Register<SceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();
         builder.Register<LoadingCurtain>(Lifetime.Singleton).AsImplementedInterfaces();

         // --- global state machine ---
         builder.Register<StateFactory>(Lifetime.Singleton).AsImplementedInterfaces();
         // States are resolved by the StateFactory by their concrete type.
         builder.Register<BootstrapState>(Lifetime.Singleton);
         builder.Register<LoadLevelState>(Lifetime.Singleton);
         builder.Register<LevelLoopState>(Lifetime.Singleton);

         // --- entry points (ticked by VContainer's player loop) ---
         builder.UseEntryPoints(entries =>
         {
            entries.Add<AnalyticService>();       // IAnalyticService + IInitializable/ITickable/IDisposable
            entries.Add<GameStateMachine>();      // IGameStateMachine + ITickable
            entries.Add<BootInitializeSystem>();  // IInitializable -> Enter<BootstrapState>
         });
      }
   }
}
