using Code.Infrastructure.Services.StateMachine;
using UnityEngine;
using VContainer.Unity;

namespace Code.GameApp.Boot
{
   /// <summary>
   /// VContainer IInitializable entry point that kicks off the global state
   /// machine into <see cref="States.BootstrapState"/>. All the real boot work
   /// (warmup, scene transition) lives in the states, not here.
   /// </summary>
   public sealed class BootInitializeSystem : IInitializable
   {
      private readonly IGameStateMachine _stateMachine;

      public BootInitializeSystem(IGameStateMachine stateMachine)
      {
         _stateMachine = stateMachine;
      }

      public void Initialize()
      {
         Debug.Log("BootInitializeSystem Initialize");
         _stateMachine.Enter<States.BootstrapState>();
      }
   }
}
