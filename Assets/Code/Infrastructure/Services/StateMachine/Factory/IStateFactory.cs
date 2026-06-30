namespace Code.Infrastructure.Services.StateMachine.Factory
{
   public interface IStateFactory
   {
      TState GetState<TState>() where TState : class, IExitableState;
   }
}
