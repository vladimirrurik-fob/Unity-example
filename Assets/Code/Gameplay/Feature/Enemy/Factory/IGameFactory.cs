using UnityEngine;

namespace Code.Gameplay.Feature.Enemy.Factory
{
  public interface IGameFactory
  {
    void CreateEnemy(GameObject prefab, Vector3 position);
    GameObject CreateBullet(GameObject prefab, Vector3 position);
  }
}