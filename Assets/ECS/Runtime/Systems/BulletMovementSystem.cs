using Unity.Entities;
using Unity.Transforms;

namespace Rts.Ecs.Systems
{
   /// <summary>Moves bullets along their velocity and destroys them when their TTL expires.</summary>
   [UpdateInGroup(typeof(SimulationSystemGroup))]
   public partial class BulletMovementSystem : SystemBase
   {
      protected override void OnUpdate()
      {
         float dt = SystemAPI.Time.DeltaTime;
         var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(World.Unmanaged);

         foreach (var (transform, velocity, ttl, entity)
                  in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Velocity>, RefRW<TimeToLive>>()
                     .WithAll<BulletTag>().WithEntityAccess())
         {
            ttl.ValueRW.Value -= dt;
            var p = transform.ValueRO.Position;
            p += velocity.ValueRO.Value * dt;
            transform.ValueRW.Position = p;

            if (ttl.ValueRO.Value <= 0f)
            {
               ecb.DestroyEntity(entity);
            }
         }
      }
   }
}
