using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Code.Gameplay.Feature.Enemy.Factory
{
  public class GameFactory : IGameFactory
  {
    private readonly IObjectResolver _resolver;

    public GameFactory(IObjectResolver resolver)
    {
      _resolver = resolver;
    }

    // The player is authored directly in the scene; its saveables (e.g. PlayerHealth)
    // are picked up by the DI container, so nothing to register here.
    public void CreatePlayer()
    {
    }

    public void CreateEnemy(GameObject prefab, Vector3 position)
    {
      _resolver.Instantiate(prefab, position, Quaternion.identity);
    }

    public GameObject CreateBullet(GameObject prefab, Vector3 position)
    {
      return _resolver.Instantiate(prefab, position, Quaternion.identity);
    }
  }
}
