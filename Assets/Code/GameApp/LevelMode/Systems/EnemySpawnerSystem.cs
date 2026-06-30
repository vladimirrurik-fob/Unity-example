using Code.Gameplay.Feature.Enemy.Factory;
using Code.Infrastructure.Services.Analytic;
using UnityEngine;
using VContainer.Unity;

namespace Code.GameApp.LevelMode.Systems
{
  public class EnemySpawnerSystem : IInitializable
  {
    private readonly IGameFactory _gameFactory;
    private readonly IAnalyticService _analytics;

    public EnemySpawnerSystem(IGameFactory gameFactory)
    {
      _gameFactory = gameFactory;
    }
    
    public void Initialize()
    {
      // _gameFactory.CreateEnemy(null, Vector3.zero);
    }
  }
}