using System;
using UnityEngine;
using VContainer.Unity;

namespace Code.Infrastructure.Services.Analytic
{
  public class AnalyticService : IAnalyticService, IInitializable, ITickable, IDisposable
  {
    private int _counter = 1;

    public AnalyticService()
    {
      
    }

    public void Warmup()
    {
      //AppMetrica.Activate();
    }
    
    public void Log(string message)
    {
      Debug.Log(message);
      // _counter++;
    }

    public void Initialize()
    {
      Debug.Log(GetType().Name + " Initialize");
    }

    public void Tick()
    {
      // Debug.Log(GetType().Name + "Tick");
    }

    public void Dispose()
    {
      Debug.Log(GetType().Name + "Dispose");
    }
  }
}