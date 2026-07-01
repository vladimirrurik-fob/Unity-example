using Unity.Entities;

namespace Rts.Ecs.Systems
{
   /// <summary>Destroys any unit whose health has dropped to zero or below.</summary>
   [UpdateInGroup(typeof(SimulationSystemGroup))]
   public partial class DeathSystem : SystemBase
   {
      protected override void OnUpdate()
      {
         var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(World.Unmanaged);

         foreach (var (health, entity)
                  in SystemAPI.Query<RefRO<Health>>().WithAll<UnitTag>().WithEntityAccess())
         {
            if (health.ValueRO.Value <= 0)
            {
               ecb.DestroyEntity(entity);
            }
         }
      }
   }
}
