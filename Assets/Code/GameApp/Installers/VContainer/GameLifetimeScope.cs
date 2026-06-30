using Code.Core.Abstractions;
using Code.GameApp.LevelMode.Systems;
using Code.GameApp.Systems;
using Code.Gameplay.Feature.Enemy;
using Code.Gameplay.Feature.Enemy.Factory;
using Code.Gameplay.Feature.Player;
using Code.Gameplay.Feature.Player.Behaviours;
using Code.Gameplay.Feature.Scene;
using Code.Infrastructure.Services._Input;
using Code.Infrastructure.Services.Analytic;
using Code.Infrastructure.Services.SaveLoad;
using Code.Infrastructure.Services.SaveLoad.Persistence;
using Code.Infrastructure.Services.SaveLoad.Progress;
using Code.Infrastructure.Services.SaveLoad.Registry;
using Code.Infrastructure.Services.SceneLoad;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Code.GameApp.Installers.VContainer
{
   /// <summary>
   /// Composition root for a gameplay scene (Level1 / Level2). Self-contained: it
   /// owns infrastructure services, the layered save/load stack, scene MonoBehaviours
   /// (injected by VContainer, no manual wiring), and the entry-point systems that
   /// drive load-on-boot and the save button.
   /// </summary>
   public sealed class GameLifetimeScope : LifetimeScope
   {
      protected override void Configure(IContainerBuilder builder)
      {
         // --- Infrastructure services ---
         builder.Register<InputService>(Lifetime.Singleton).AsImplementedInterfaces();
         builder.Register<SceneLoader>(Lifetime.Singleton).AsImplementedInterfaces();
         builder.Register<GameFactory>(Lifetime.Singleton).AsImplementedInterfaces();
         builder.Register<PlayerService>(Lifetime.Singleton).As<IPlayerService>();

         // --- Save/load stack (layered, file-backed) ---
         builder.Register<IProgressRepository, FileProgressRepository>(Lifetime.Singleton);
         builder.Register<ProgressService>(Lifetime.Singleton).AsImplementedInterfaces();
         builder.Register<SaveLoadRegistry>(Lifetime.Singleton).AsImplementedInterfaces();
         builder.Register<SaveLoadService>(Lifetime.Singleton).AsImplementedInterfaces();

         // --- ISaveLoad adapters (each registered as ISaveLoad so the registry collects them) ---
         // Player position: resolved off the PlayerFacade from the scene.
         if (Present<PlayerFacade>())
         {
            builder.Register<PlayerPositionSaveLoad>(Lifetime.Singleton).AsImplementedInterfaces();
         }

         // --- Scene MonoBehaviours that need DI injection. Registered only when
         // present: gameplay scenes differ (Level2 has no SceneSwitcher/enemies),
         // and RegisterComponentInHierarchy throws if a type is missing. ---
         RegisterIfPresent<PlayerMovement>(builder);
         RegisterIfPresent<global::Hud>(builder);
         RegisterIfPresent<SceneSwitcher>(builder);
         RegisterIfPresent<PlayerFacade>(builder);
         RegisterIfPresent<EnemyHealth>(builder);
         if (Present<PlayerHealth>())
         {
            // PlayerHealth is also the ISaveLoad adapter for player data.
            builder.RegisterComponentInHierarchy<PlayerHealth>().AsImplementedInterfaces();
         }

         // --- Entry-point systems (ticked by VContainer's player loop) ---
         builder.UseEntryPoints(entries =>
         {
            entries.Add<AnalyticService>();      // IAnalyticService + tickable
            entries.Add<EnemySpawnerSystem>();   // IInitializable
            entries.Add<SaveLoadInitSystem>();   // hydrate progress from file on boot
            entries.Add<SaveLoadSystem>();       // wire save/load buttons + apply loaded data
         });
      }

      private static bool Present<T>() where T : Component =>
         Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 0;

      private static void RegisterIfPresent<T>(IContainerBuilder builder) where T : Component
      {
         if (Present<T>())
         {
            builder.RegisterComponentInHierarchy<T>();
         }
      }
   }
}
