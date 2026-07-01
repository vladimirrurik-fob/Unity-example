using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Rts.Ecs
{
   /// <summary>
   /// Debug/preview visualizer: mirrors the pure-ECS battle onto regular GameObjects
   /// (MeshRenderer) each frame. This is ONLY a view layer — the simulation stays 100%
   /// ECS (entities + systems). Used because Entities Graphics (BatchRendererGroup) does
   /// not draw in this Unity 6/URP environment, while normal MeshRenderer rendering works.
   /// </summary>
   public sealed class RtsHybridVisualizer : MonoBehaviour
   {
      [Header("Unit visuals")]
      [SerializeField] private Mesh _unitMesh;
      [SerializeField] private Material _redMaterial;
      [SerializeField] private Material _blueMaterial;
      [Header("Bullet visuals")]
      [SerializeField] private Mesh _bulletMesh;
      [SerializeField] private Material _bulletRedMaterial;
      [SerializeField] private Material _bulletBlueMaterial;
      [Header("Pool sizes")]
      [SerializeField] private int _maxUnits = 220;
      [SerializeField] private int _maxBullets = 400;

      private GameObject[] _units;
      private GameObject[] _bullets;
      private EntityQuery _unitQuery;
      private EntityQuery _bulletQuery;

      private void Start()
      {
         _units = BuildPool("Unit_", _maxUnits, _unitMesh, 1.4f);
         _bullets = BuildPool("Bullet_", _maxBullets, _bulletMesh, 0.4f);
      }

      private void Update()
      {
         var world = World.DefaultGameObjectInjectionWorld;
         if (world == null)
         {
            return;
         }

         var em = world.EntityManager;
         if (_unitQuery == default)
         {
            _unitQuery = em.CreateEntityQuery(ComponentType.ReadOnly<UnitTag>(), ComponentType.ReadOnly<Team>(), ComponentType.ReadOnly<LocalTransform>());
            _bulletQuery = em.CreateEntityQuery(ComponentType.ReadOnly<BulletTag>(), ComponentType.ReadOnly<Team>(), ComponentType.ReadOnly<LocalTransform>());
         }

         Sync(_unitQuery, _units, em);
         Sync(_bulletQuery, _bullets, em);
      }

      private void Sync(EntityQuery query, GameObject[] pool, EntityManager em)
      {
         if (query.CalculateEntityCount() == 0)
         {
            for (int i = 0; i < pool.Length; i++) pool[i].SetActive(false);
            return;
         }

         var teams = query.ToComponentDataArray<Team>(Allocator.Temp);
         var transforms = query.ToComponentDataArray<LocalTransform>(Allocator.Temp);
         int n = teams.Length;
         for (int i = 0; i < pool.Length; i++)
         {
            if (i < n)
            {
               bool red = teams[i].Value == 0;
               pool[i].transform.position = transforms[i].Position;
               pool[i].SetActive(true);
               var mr = pool[i].GetComponent<MeshRenderer>();
               var mat = pool == _units ? (red ? _redMaterial : _blueMaterial) : (red ? _bulletRedMaterial : _bulletBlueMaterial);
               if (mr.sharedMaterial != mat) mr.sharedMaterial = mat;
            }
            else
            {
               pool[i].SetActive(false);
            }
         }
         teams.Dispose();
         transforms.Dispose();
      }

      private GameObject[] BuildPool(string prefix, int count, Mesh mesh, float scale)
      {
         var pool = new GameObject[count];
         var parent = new GameObject(prefix + "Pool");
         parent.transform.SetParent(transform, false);
         for (int i = 0; i < count; i++)
         {
            var go = new GameObject(prefix + i, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.SetParent(parent.transform, false);
            go.transform.localScale = Vector3.one * scale;
            go.GetComponent<MeshFilter>().sharedMesh = mesh;
            go.SetActive(false);
            pool[i] = go;
         }
         return pool;
      }
   }
}
