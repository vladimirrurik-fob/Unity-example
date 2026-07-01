using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rts.Ecs
{
   /// <summary>
   /// Scene entry point: registers the warrior mesh and the four team materials with
   /// Entities Graphics, then writes the <see cref="RtsConfig"/> singleton that the
   /// ECS systems read. The armies themselves are spawned by UnitSpawnerSystem.
   /// </summary>
   public sealed class RtsBootstrapper : MonoBehaviour
   {
      [Header("Rendering")]
      [SerializeField] private Mesh _unitMesh;
      [SerializeField] private Mesh _bulletMesh;
      [SerializeField] private Material _unitRed;
      [SerializeField] private Material _unitBlue;
      [SerializeField] private Material _bulletRed;
      [SerializeField] private Material _bulletBlue;

      [Header("Simulation")]
      [SerializeField] private int _unitsPerTeam = 100;
      [SerializeField] private float _unitMoveSpeed = 3f;
      [SerializeField] private float _bulletSpeed = 20f;
      [SerializeField] private float _attackRange = 9f;
      [SerializeField] private float _shootInterval = 0.7f;
      [SerializeField] private float _unitSpacing = 1.5f;

      private void Start()
      {
         // Ensure the default Entities world exists (auto-init can be skipped depending on settings).
         if (World.DefaultGameObjectInjectionWorld == null)
         {
            DefaultWorldInitialization.Initialize("Default World");
         }

         var world = World.DefaultGameObjectInjectionWorld;
         if (world == null)
         {
            Debug.LogError("[RTS] No default Entities world could be created.");
            return;
         }

         var graphics = world.GetOrCreateSystemManaged<EntitiesGraphicsSystem>();

         // Warriors use a renderable capsule mesh (assigned asset, or builtin fallback).
         // (The SciFiWarrior asset is a skinned mesh, which Entities Graphics can't render
         // without full skinning setup.)
         var unitMesh = ResolveMesh(_unitMesh, "Capsule.fbx");
         var bulletMesh = ResolveMesh(_bulletMesh, "Sphere.fbx");

         var em = world.EntityManager;
         var configEntity = em.CreateEntity();
         em.AddComponentData(configEntity, new RtsConfig
         {
            UnitsPerTeam = _unitsPerTeam,
            UnitMoveSpeed = _unitMoveSpeed,
            BulletSpeed = _bulletSpeed,
            AttackRange = _attackRange,
            ShootInterval = _shootInterval,
            UnitSpacing = _unitSpacing,
            Spawned = false,
            UnitRedMat = graphics.RegisterMaterial(_unitRed),
            UnitBlueMat = graphics.RegisterMaterial(_unitBlue),
            BulletRedMat = graphics.RegisterMaterial(_bulletRed),
            BulletBlueMat = graphics.RegisterMaterial(_bulletBlue),
            UnitMesh = graphics.RegisterMesh(unitMesh),
            BulletMesh = graphics.RegisterMesh(bulletMesh),
         });

         Debug.Log($"[RTS] Bootstrapped: {_unitsPerTeam * 2} warriors registered. unitMesh={unitMesh!=null} bulletMesh={bulletMesh!=null}");
      }

      private static Mesh ResolveMesh(Mesh assigned, string builtinName)
      {
         if (assigned != null)
         {
            return assigned;
         }

         var builtin = Resources.GetBuiltinResource<Mesh>(builtinName);
         if (builtin != null)
         {
            return builtin;
         }

         // Last-resort: derive a mesh from a primitive GameObject.
         var prim = GameObject.CreatePrimitive(builtinName == "Sphere.fbx"
            ? PrimitiveType.Sphere : PrimitiveType.Capsule);
         var mesh = prim.GetComponent<MeshFilter>().sharedMesh;
         Destroy(prim);
         return mesh;
      }
   }
}
