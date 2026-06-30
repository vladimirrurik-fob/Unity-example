using VContainer;

namespace Code.Infrastructure.Services.StateMachine.Factory
{
   /// <summary>
   /// Resolves state instances from the VContainer <see cref="IObjectResolver"/>.
   /// (In the Zenject lecture this used an IDiService service-locator; here the
   /// container is injected directly — no global locator needed.)
   /// </summary>
   public sealed class StateFactory : IStateFactory
   {
      private readonly IObjectResolver _resolver;

      public StateFactory(IObjectResolver resolver)
      {
         _resolver = resolver;
      }

      public TState GetState<TState>() where TState : class, IExitableState =>
         _resolver.Resolve<TState>();
   }
}
