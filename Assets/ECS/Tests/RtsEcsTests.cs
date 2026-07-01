using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Rts.Ecs;
using Rts.Ecs.Systems;

namespace Rts.Ecs.Tests
{
   /// <summary>
   /// EditMode tests that drive the RTS systems in an isolated World (no play loop,
   /// no rendering) and assert the battle mechanics: movement, damage and death.
   /// </summary>
   public class RtsEcsTests
   {
      private World _world;
      private EntityManager _em;
      private SimulationSystemGroup _sim;
      private double _elapsed;

      [SetUp]
      public void SetUp()
      {
         _world = new World("RtsTest");
         _em = _world.EntityManager;
         _sim = _world.GetOrCreateSystemManaged<SimulationSystemGroup>();
         // ECB systems provide the Singletons the systems look up via SystemAPI.GetSingleton.
         _world.GetOrCreateSystemManaged<BeginSimulationEntityCommandBufferSystem>();
         _sim.AddSystemToUpdateList(_world.GetOrCreateSystemManaged<EndSimulationEntityCommandBufferSystem>());
      }

      [TearDown]
      public void TearDown()
      {
         _world?.Dispose();
      }

      private void Tick(int frames, float dt = 0.1f)
      {
         double elapsed = _world.Time.ElapsedTime;
         for (int i = 0; i < frames; i++)
         {
            elapsed += dt;
            _world.SetTime(new Unity.Core.TimeData(elapsed, dt));
            _world.Update();
         }
      }

      private void AddToSim<T>() where T : SystemBase =>
         _sim.AddSystemToUpdateList(_world.GetOrCreateSystemManaged<T>());

      private Entity CreateUnit(byte team, float x, int health = 10)
      {
         var e = _em.CreateEntity(
            typeof(UnitTag), typeof(Team), typeof(Health), typeof(MoveSpeed),
            typeof(Target), typeof(LocalTransform), typeof(LocalToWorld));
         _em.SetComponentData(e, new Team { Value = team });
         _em.SetComponentData(e, new Health { Value = health });
         _em.SetComponentData(e, new MoveSpeed { Value = 3f });
         _em.SetComponentData(e, new Target { InRange = false });
         _em.SetComponentData(e, LocalTransform.FromPosition(new float3(x, 0f, 0f)));
         return e;
      }

      [Test]
      public void Units_MarchTowardTheEnemy()
      {
         AddToSim<UnitMovementSystem>();
         var red = CreateUnit(0, -10f);   // Red marches +X
         var blue = CreateUnit(1, 10f);   // Blue marches -X

         Tick(30);

         float redX = _em.GetComponentData<LocalTransform>(red).Position.x;
         float blueX = _em.GetComponentData<LocalTransform>(blue).Position.x;
         Assert.Greater(redX, -10f, "red should have advanced toward +X");
         Assert.Less(blueX, 10f, "blue should have advanced toward -X");
         Assert.Less(blueX - redX, 20f, "armies should have closed distance");
      }

      [Test]
      public void Bullet_HitDealsOneDamage()
      {
         AddToSim<BulletCollisionSystem>();
         var enemy = CreateUnit(1, 0f, health: 5); // blue unit

         var bullet = _em.CreateEntity(
            typeof(BulletTag), typeof(Team), typeof(LocalTransform), typeof(LocalToWorld));
         _em.SetComponentData(bullet, new Team { Value = 0 }); // red bullet
         _em.SetComponentData(bullet, LocalTransform.FromPosition(new float3(0f, 0f, 0f)));

         Tick(1);

         Assert.AreEqual(4, _em.GetComponentData<Health>(enemy).Value, "bullet should deal 1 damage");
         Assert.IsFalse(_em.Exists(bullet), "bullet should be destroyed on hit");
      }

      [Test]
      public void Unit_WithZeroHealthIsDestroyed()
      {
         AddToSim<DeathSystem>();
         var unit = CreateUnit(0, 0f, health: 0);

         Tick(1);

         Assert.IsFalse(_em.Exists(unit), "dead unit should be destroyed");
      }
   }
}
