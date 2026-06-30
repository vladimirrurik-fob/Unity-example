using Code.Gameplay.Feature.Enemy;
using UnityEngine;

namespace Code.Gameplay.Feature.Player.Behaviours
{
  public class Bullet : MonoBehaviour
  {
    private void OnTriggerEnter(Collider other)
    {
      if (other.TryGetComponent(out IEnemyHealth bossHealth))
        bossHealth.TakeDamage(10);
      {
      }
    }
  }
}