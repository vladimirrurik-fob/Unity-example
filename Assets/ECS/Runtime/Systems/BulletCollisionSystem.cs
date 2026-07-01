using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Rts.Ecs.Systems
{
   /// <summary>When a bullet reaches an enemy unit (opposite team, within hit radius)
   /// it deals 1 damage and is destroyed.</summary>
   [UpdateInGroup(typeof(SimulationSystemGroup))]
   public partial class BulletCollisionSystem : SystemBase
   {
      private EntityQuery _units;

      protected override void OnCreate()
      {
         _units = GetEntityQuery(
            ComponentType.ReadOnly<UnitTag>(),
            ComponentType.ReadOnly<Team>(),
            ComponentType.ReadOnly<LocalTransform>(),
            ComponentType.ReadOnly<Health>());
      }

      protected override void OnUpdate()
      {
         int n = _units.CalculateEntityCount();
         if (n == 0)
         {
            return;
         }

         const float hitRadiusSq = 1.0f;

         var uEntity = _units.ToEntityArray(Allocator.Temp);
         var uTeam = _units.ToComponentDataArray<Team>(Allocator.Temp);
         var uTransform = _units.ToComponentDataArray<LocalTransform>(Allocator.Temp);
         var uHealth = _units.ToComponentDataArray<Health>(Allocator.Temp);

         var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(World.Unmanaged);

         foreach (var (transform, team, entity)
                  in SystemAPI.Query<RefRO<LocalTransform>, RefRO<Team>>()
                     .WithAll<BulletTag>().WithEntityAccess())
         {
            float3 bp = transform.ValueRO.Position;
            byte bt = team.ValueRO.Value;

            for (int j = 0; j < n; j++)
            {
               if (uTeam[j].Value == bt)
               {
                  continue;
               }
               if (math.distancesq(bp, uTransform[j].Position) <= hitRadiusSq)
               {
                  ecb.SetComponent(uEntity[j], new Health { Value = uHealth[j].Value - 1 });
                  ecb.DestroyEntity(entity);
                  break;
               }
            }
         }

         uEntity.Dispose();
         uTeam.Dispose();
         uTransform.Dispose();
         uHealth.Dispose();
      }
   }
}
