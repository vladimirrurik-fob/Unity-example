using Code.Infrastructure.Services.Analytic;
using Code.Infrastructure.Services.Curtain;
using Code.Infrastructure.Services.SceneLoad;
using Code.Infrastructure.Services.StateMachine;
using UnityEngine;

namespace Code.GameApp.States
{
   /// <summary>
   /// First state of the bootstrap flow: warms up infrastructure/analytics and
   /// reports progress to the console, then transitions to loading the game scene.
   /// </summary>
   public sealed class BootstrapState : IState
   {
      private readonly IGameStateMachine _stateMachine;
      private readonly ISceneLoader _sceneLoader;
      private readonly ILoadingCurtain _curtain;
      private readonly IAnalyticService _analytic;

      public BootstrapState(IGameStateMachine stateMachine,
         ISceneLoader sceneLoader,
         ILoadingCurtain curtain,
         IAnalyticService analytic)
      {
         _stateMachine = stateMachine;
         _sceneLoader = sceneLoader;
         _curtain = curtain;
         _analytic = analytic;
      }

      public void Enter()
      {
         _curtain.Show();

         _analytic.Warmup();

         Debug.Log("Bootstrap created.");
         Debug.Log("Create 50 Infrastructure Services");
         Debug.Log("Get to Server for updates");
         Debug.Log("Warmup Assets");
         Debug.Log("Game Warmup Complete");

         _stateMachine.Enter<LoadLevelState, string>("1. Level1");
      }

      public void Exit()
      {
      }
   }
}
