using Unity.Entities;
using Unity.Entities.Graphics;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

namespace Rts.Ecs.Systems
{
   /// <summary>One-time spawn of two armies: Red on the left (marches +X),
   /// Blue on the right (marches -X).</summary>
   [UpdateInGroup(typeof(SimulationSystemGroup))]
   public partial class UnitSpawnerSystem : SystemBase
   {
      protected override void OnUpdate()
      {
         if (!SystemAPI.HasSingleton<RtsConfig>())
         {
            return;
         }

         ref var cfg = ref SystemAPI.GetSingletonRW<RtsConfig>().ValueRW;
         if (cfg.Spawned)
         {
            return;
         }

         cfg.Spawned = true;
         var em = EntityManager;

         var arch = em.CreateArchetype(
            typeof(UnitTag), typeof(Team), typeof(Health), typeof(MoveSpeed),
            typeof(ShootTimer), typeof(Target), typeof(LocalTransform), typeof(LocalToWorld),
            typeof(MaterialMeshInfo), typeof(RenderBounds), typeof(RenderFilterSettings));

         var bounds = new RenderBounds { Value = new AABB { Center = float3.zero, Extents = new float3(2f) } };
         int cols = 10;
         int rows = cfg.UnitsPerTeam / cols;
         float scale = 0.7f;

         SpawnTeam(em, arch, in cfg, team: 0, centerX: -20f, mat: cfg.UnitRedMat, cols, rows, cfg.UnitSpacing, scale, cfg.UnitsPerTeam, bounds);
         SpawnTeam(em, arch, in cfg, team: 1, centerX:  20f, mat: cfg.UnitBlueMat, cols, rows, cfg.UnitSpacing, scale, cfg.UnitsPerTeam, bounds);
      }

      private static void SpawnTeam(EntityManager em, EntityArchetype arch, in RtsConfig cfg,
         byte team, float centerX, BatchMaterialID mat, int cols, int rows, float spacing, float scale, int count, RenderBounds bounds)
      {
         for (int i = 0; i < count; i++)
         {
            int c = i % cols;
            int r = i / cols;
            var e = em.CreateEntity(arch);
            em.SetComponentData(e, new Team { Value = team });
            em.SetComponentData(e, new Health { Value = 10 });
            em.SetComponentData(e, new MoveSpeed { Value = cfg.UnitMoveSpeed });
            em.SetComponentData(e, new ShootTimer { Value = cfg.ShootInterval });
            em.SetComponentData(e, new Target { InRange = false });
            float x = centerX + (c - cols * 0.5f) * spacing;
            float z = (r - rows * 0.5f) * spacing;
            em.SetComponentData(e, LocalTransform.FromPositionRotationScale(
               new float3(x, 0f, z), quaternion.identity, scale));
            em.SetComponentData(e, new MaterialMeshInfo(mat, cfg.UnitMesh));
            em.SetComponentData(e, bounds);
         }
      }
   }
}
