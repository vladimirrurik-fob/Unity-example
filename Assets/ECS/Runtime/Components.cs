using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine.Rendering;

namespace Rts.Ecs
{
   /// <summary>Team identifier. 0 = Red, 1 = Blue.</summary>
   public struct Team : IComponentData
   {
      public byte Value;
   }

   /// <summary>Current health of a unit. -1 per bullet hit; destroyed at 0.</summary>
   public struct Health : IComponentData
   {
      public int Value;
   }

   public struct MoveSpeed : IComponentData
   {
      public float Value;
   }

   /// <summary>Time remaining before the unit can fire again.</summary>
   public struct ShootTimer : IComponentData
   {
      public float Value;
   }

   public struct UnitTag : IComponentData { }

   public struct BulletTag : IComponentData { }

   public struct Damage : IComponentData
   {
      public int Value;
   }

   public struct Velocity : IComponentData
   {
      public float3 Value;
   }

   public struct TimeToLive : IComponentData
   {
      public float Value;
   }

   /// <summary>Result of targeting: the nearest enemy and a cached aim position.</summary>
   public struct Target : IComponentData
   {
      public Entity Entity;
      public float3 Position;
      public bool InRange;
   }

   /// <summary>
   /// Global simulation config + the runtime rendering batch IDs registered by the
   /// bootstrapper. A singleton entity carries it.
   /// </summary>
   public struct RtsConfig : IComponentData
   {
      public int UnitsPerTeam;
      public float UnitMoveSpeed;
      public float BulletSpeed;
      public float AttackRange;
      public float ShootInterval;
      public float UnitSpacing;
      public bool Spawned;

      public BatchMaterialID UnitRedMat;
      public BatchMaterialID UnitBlueMat;
      public BatchMaterialID BulletRedMat;
      public BatchMaterialID BulletBlueMat;
      public BatchMeshID UnitMesh;
      public BatchMeshID BulletMesh;
   }
}
