using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Rts.Ecs.Systems
{
   /// <summary>For every unit, finds the nearest enemy. If within attack range it
   /// marks Target.InRange so the unit stops and shoots.</summary>
   [UpdateInGroup(typeof(SimulationSystemGroup))]
   public partial class TargetingSystem : SystemBase
   {
      private EntityQuery _units;

      protected override void OnCreate()
      {
         _units = GetEntityQuery(
            ComponentType.ReadOnly<UnitTag>(),
            ComponentType.ReadOnly<Team>(),
            ComponentType.ReadOnly<LocalTransform>(),
            ComponentType.ReadOnly<Target>());
         RequireForUpdate<RtsConfig>();
      }

      protected override void OnUpdate()
      {
         int n = _units.CalculateEntityCount();
         if (n == 0)
         {
            return;
         }

         var cfg = SystemAPI.GetSingleton<RtsConfig>();
         float rangeSq = cfg.AttackRange * cfg.AttackRange;

         var entities = _units.ToEntityArray(Allocator.Temp);
         var teams = _units.ToComponentDataArray<Team>(Allocator.Temp);
         var transforms = _units.ToComponentDataArray<LocalTransform>(Allocator.Temp);

         var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(World.Unmanaged);

         for (int i = 0; i < n; i++)
         {
            float3 pi = transforms[i].Position;
            byte ti = teams[i].Value;
            float bestSq = float.MaxValue;
            int best = -1;
            for (int j = 0; j < n; j++)
            {
               if (teams[j].Value == ti)
               {
                  continue;
               }
               float d = math.distancesq(pi, transforms[j].Position);
               if (d < bestSq)
               {
                  bestSq = d;
                  best = j;
               }
            }

            var target = new Target { InRange = false };
            if (best >= 0)
            {
               target.Entity = entities[best];
               target.Position = transforms[best].Position;
               target.InRange = bestSq <= rangeSq;
            }
            ecb.SetComponent(entities[i], target);
         }

         entities.Dispose();
         teams.Dispose();
         transforms.Dispose();
      }
   }
}
