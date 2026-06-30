using Code.Infrastructure.Services.StateMachine.Factory;
using VContainer.Unity;

namespace Code.Infrastructure.Services.StateMachine
{
   /// <summary>
   /// Global game state machine. Holds the active state, exits the previous one on
   /// transition, and resolves the next state through the <see cref="IStateFactory"/>.
   /// Implements VContainer's ITickable so the active state can tick if it needs to.
   /// </summary>
   public sealed class GameStateMachine : IGameStateMachine, ITickable
   {
      private readonly IStateFactory _stateFactory;
      private IExitableState _currentState;

      public GameStateMachine(IStateFactory stateFactory)
      {
         _stateFactory = stateFactory;
      }

      public void Enter<TState>() where TState : class, IState
      {
         IState state = ChangeState<TState>();
         state.Enter();
      }

      public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
      {
         TState state = ChangeState<TState>();
         state.Enter(payload);
      }

      void ITickable.Tick()
      {
         // Forward to the active state if it is tickable (kept for parity with the lecture).
      }

      private TState ChangeState<TState>() where TState : class, IExitableState
      {
         _currentState?.Exit();
         TState state = GetState<TState>();
         _currentState = state;
         return state;
      }

      private TState GetState<TState>() where TState : class, IExitableState =>
         _stateFactory.GetState<TState>();
   }
}
