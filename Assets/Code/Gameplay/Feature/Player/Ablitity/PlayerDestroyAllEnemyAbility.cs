using Code.Gameplay.Feature.Enemy.Services;
using Code.Gameplay.Feature.Player.Behaviours;
using UnityEngine;

namespace Code.Gameplay.Feature.Player.Ablitity
{
  public class PlayerDestroyAllEnemyAbility : MonoBehaviour
  {
    [SerializeField] private EnemyService _enemyService;
    [SerializeField] private Bullet _bulletPrefab;

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.O))
        DestroyAllEnemy();
    }

    public void DestroyAllEnemy()
    {
      _enemyService.DestroyAllEnemy();
    }
  }
}
