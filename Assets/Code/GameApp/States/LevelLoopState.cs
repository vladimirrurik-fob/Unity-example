using Code.Infrastructure.Services.StateMachine;
using UnityEngine;

namespace Code.GameApp.States
{
   /// <summary>
   /// Active while a gameplay level runs.
   /// </summary>
   public sealed class LevelLoopState : IState
   {
      private readonly IGameStateMachine _stateMachine;

      public LevelLoopState(IGameStateMachine stateMachine)
      {
         _stateMachine = stateMachine;
      }

      public void Enter()
      {
         Debug.Log("Enter Level Loop State");
      }

      public void Exit()
      {
      }
   }
}
