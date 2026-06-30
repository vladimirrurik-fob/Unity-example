using Code.Infrastructure.Services.Analytic;
using Code.Infrastructure.Services.SceneLoad;
using UnityEngine;
using VContainer.Unity;

namespace Code.GameApp.Boot
{
  public class BootInitializeSystem : IInitializable
  {
    private readonly IAnalyticService _analytics;
    private readonly ISceneLoader _sceneLoader;

    public BootInitializeSystem(IAnalyticService analytics,
      ISceneLoader sceneLoader)
    {
      _analytics = analytics;
      _sceneLoader = sceneLoader;
    }
      
    public void Initialize()
    { 
      _analytics.Warmup();
      _sceneLoader.LoadScene("1. Level1");
      Debug.Log("BootInitializeSystem Initialize");
    }
  }
}