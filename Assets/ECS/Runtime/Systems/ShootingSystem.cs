using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

namespace Rts.Ecs.Systems
{
   /// <summary>Units with an enemy in range fire a team-coloured bullet toward it on a cooldown.</summary>
   [UpdateInGroup(typeof(SimulationSystemGroup))]
   public partial class ShootingSystem : SystemBase
   {
      protected override void OnCreate() => RequireForUpdate<RtsConfig>();

      protected override void OnUpdate()
      {
         var cfg = SystemAPI.GetSingleton<RtsConfig>();
         float dt = SystemAPI.Time.DeltaTime;
         var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(World.Unmanaged);
         var bounds = new RenderBounds { Value = new AABB { Center = float3.zero, Extents = new float3(0.5f) } };

         foreach (var (transform, team, target, timer)
                  in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Team>, RefRO<Target>, RefRW<ShootTimer>>()
                     .WithAll<UnitTag>())
         {
            timer.ValueRW.Value -= dt;
            if (!target.ValueRO.InRange || timer.ValueRO.Value > 0f)
            {
               continue;
            }

            timer.ValueRW.Value = cfg.ShootInterval;
            float3 dir = math.normalizesafe(target.ValueRO.Position - transform.ValueRO.Position);

            var e = ecb.CreateEntity();
            ecb.AddComponent(e, new BulletTag());
            ecb.AddComponent(e, new Team { Value = team.ValueRO.Value });
            ecb.AddComponent(e, new Velocity { Value = dir * cfg.BulletSpeed });
            ecb.AddComponent(e, new Damage { Value = 1 });
            ecb.AddComponent(e, new TimeToLive { Value = 3f });
            ecb.AddComponent(e, LocalTransform.FromPositionRotationScale(transform.ValueRO.Position, quaternion.identity, 0.3f));
            ecb.AddComponent(e, new LocalToWorld());
            ecb.AddComponent(e, new MaterialMeshInfo(
               team.ValueRO.Value == 0 ? cfg.BulletRedMat : cfg.BulletBlueMat, cfg.BulletMesh));
            ecb.AddComponent(e, bounds);
            ecb.AddSharedComponent(e, new RenderFilterSettings());
         }
      }
   }
}
