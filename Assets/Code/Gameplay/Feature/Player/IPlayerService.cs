using Code.Gameplay.Feature.Player.Behaviours;

namespace Code.Gameplay.Feature.Player
{
   public interface IPlayerService
   {
      PlayerFacade PlayerFacade { get; }
      void Register(PlayerFacade playerFacade);
   }
}