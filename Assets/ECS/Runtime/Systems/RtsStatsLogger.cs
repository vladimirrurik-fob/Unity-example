using Unity.Entities;
using UnityEngine;

namespace Rts.Ecs.Systems
{
   /// <summary>Logs army/bullet counts every second of sim time — so the standalone
   /// player log shows the battle progressing (units decreasing as they die).</summary>
   [UpdateInGroup(typeof(SimulationSystemGroup))]
   public partial class RtsStatsLogger : SystemBase
   {
      private float _timer;

      protected override void OnUpdate()
      {
         _timer -= SystemAPI.Time.DeltaTime;
         if (_timer > 0f)
         {
            return;
         }

         _timer = 1f;
         int units = GetEntityQuery(ComponentType.ReadOnly<UnitTag>()).CalculateEntityCount();
         int bullets = GetEntityQuery(ComponentType.ReadOnly<BulletTag>()).CalculateEntityCount();
         Debug.Log($"[RTS] t={SystemAPI.Time.ElapsedTime:0.0}s  units={units}  bullets={bullets}");
      }
   }
}
