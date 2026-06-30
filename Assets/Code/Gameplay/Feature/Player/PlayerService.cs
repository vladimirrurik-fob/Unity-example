using Code.Core.Abstractions;
using Code.Core.Data;
using Code.Gameplay.Feature.Player.Behaviours;
using UnityEngine;

namespace Code.Gameplay.Feature.Player
{
   public class PlayerService : IPlayerService, ISaveLoad
   {
      private PlayerFacade _playerFacade;

      public void Register(PlayerFacade playerFacade)
      {
         _playerFacade = playerFacade;
      }

      public PlayerFacade PlayerFacade => _playerFacade;

      public void Save(PlayerProgress progress)
      {
         Debug.Log("PLAYER_SERVICE.SAVE");
         progress.PlayerData.CurrentHealth = _playerFacade.PlayerHealth.GetCurrentHealth();
      }

      public void Load(PlayerProgress progress)
      {
         Debug.Log("PlayerService.Load");
         _playerFacade.PlayerHealth.SetCurrentHealth(progress.PlayerData.CurrentHealth);
      }

      public void Reset(PlayerProgress progress)
      {
      }
   }
}