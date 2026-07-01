using Unity.Entities;
using UnityEngine;

namespace Rts.Ecs
{
   /// <summary>
   /// Safety net: drives the default Entities World from the regular Unity update
   /// loop. If the Entities player-loop hook is already advancing the world, this
   /// does nothing (avoids double-ticking); if it isn't, this keeps the simulation
   /// running. Place on any GameObject in the scene.
   /// </summary>
   public sealed class RtsWorldTicker : MonoBehaviour
   {
      private double _lastElapsed = -1;

      private void Update()
      {
         var world = World.DefaultGameObjectInjectionWorld;
         if (world == null)
         {
            return;
         }

         // Entities hook already advanced the world this frame -> don't double-tick.
         if (world.Time.ElapsedTime > _lastElapsed)
         {
            _lastElapsed = world.Time.ElapsedTime;
            return;
         }

         world.Update();
         _lastElapsed = world.Time.ElapsedTime;
      }
   }
}
