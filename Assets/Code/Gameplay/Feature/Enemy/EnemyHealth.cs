using Code.Gameplay.Feature.Enemy.Services;
using Code.Infrastructure.Services.Analytic;
using UnityEngine;
using VContainer;

namespace Code.Gameplay.Feature.Enemy
{
  public class EnemyHealth : MonoBehaviour, IEnemyHealth
  {
    [SerializeField] private EnemyService _enemyService;
    private IAnalyticService _analytic;

    [Inject]
    public void Construct(IAnalyticService analytic)
    {
      _analytic = analytic;
    }

    private void Awake()
    {
      _analytic?.Log("EnemyHealth Awake " + gameObject.name);
    }

    public void TakeDamage(int damage)
    {
    }

    public void Destroy()
    {
      Destroy(gameObject);
    }
  }
}
