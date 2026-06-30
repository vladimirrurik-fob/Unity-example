using Code.Infrastructure.Services.Curtain;
using Code.Infrastructure.Services.SceneLoad;
using Code.Infrastructure.Services.StateMachine;

namespace Code.GameApp.States
{
   /// <summary>
   /// Loads the requested gameplay scene, then hands control to the level loop.
   /// Payload is the scene name.
   /// </summary>
   public sealed class LoadLevelState : IPayloadedState<string>
   {
      private readonly IGameStateMachine _stateMachine;
      private readonly ISceneLoader _sceneLoader;
      private readonly ILoadingCurtain _curtain;

      public LoadLevelState(IGameStateMachine stateMachine,
         ISceneLoader sceneLoader,
         ILoadingCurtain curtain)
      {
         _stateMachine = stateMachine;
         _sceneLoader = sceneLoader;
         _curtain = curtain;
      }

      public void Enter(string sceneName)
      {
         _sceneLoader.LoadScene(sceneName);
         _curtain.Hide();
         _stateMachine.Enter<LevelLoopState>();
      }

      public void Exit()
      {
         // e.g. _saveLoad.Save(); _assets.Cleanup();
      }
   }
}
