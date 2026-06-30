using Code.Core.Abstractions;
using Code.Core.Data;
using Code.Gameplay.Feature.Player.Behaviours;
using UnityEngine;

namespace Code.Gameplay.Feature.Player
{
   /// <summary>
   /// ISaveLoad adapter that persists the player's world position. Pure C# (resolved
   /// from the container, no extra MonoBehaviour); it reads the position off the
   /// registered <see cref="PlayerFacade"/>. The CharacterController is briefly
   /// disabled while teleporting on load so it doesn't override the new position.
   /// </summary>
   public sealed class PlayerPositionSaveLoad : ISaveLoad
   {
      private readonly PlayerFacade _facade;

      public PlayerPositionSaveLoad(PlayerFacade facade)
      {
         _facade = facade;
      }

      public void Save(PlayerProgress progress)
      {
         progress.PlayerData.Position = _facade.transform.position;
      }

      public void Load(PlayerProgress progress)
      {
         var controller = _facade.GetComponent<CharacterController>();
         if (controller != null)
         {
            controller.enabled = false;
         }

         _facade.transform.position = progress.PlayerData.Position;

         if (controller != null)
         {
            controller.enabled = true;
         }

         Debug.Log($"[SaveLoad] Player position restored to {progress.PlayerData.Position}");
      }

      public void Reset(PlayerProgress progress)
      {
      }
   }
}
