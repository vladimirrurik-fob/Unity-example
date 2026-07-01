using Unity.Entities;
using Unity.Transforms;

namespace Rts.Ecs.Systems
{
   /// <summary>Marches units toward the enemy side (Red +X, Blue -X) while no enemy
   /// is in range; otherwise they stop to fight.</summary>
   [UpdateInGroup(typeof(SimulationSystemGroup))]
   public partial class UnitMovementSystem : SystemBase
   {
      protected override void OnUpdate()
      {
         float dt = SystemAPI.Time.DeltaTime;
         foreach (var (transform, team, target, speed)
                  in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Team>, RefRO<Target>, RefRO<MoveSpeed>>()
                     .WithAll<UnitTag>())
         {
            if (target.ValueRO.InRange)
            {
               continue;
            }

            float dir = team.ValueRO.Value == 0 ? 1f : -1f;
            var p = transform.ValueRO.Position;
            p.x += dir * speed.ValueRO.Value * dt;
            transform.ValueRW.Position = p;
         }
      }
   }
}
