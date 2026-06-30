using System;
using UnityEngine;

namespace Code.Infrastructure.Services.Analytic
{
  public class AnalyticManager : MonoBehaviour
  {
    public static AnalyticManager Instance { get; private set; }
    
    
    private void Awake()
    {
      Instance = this;
    }

    public void Log(string message)
    {
      
    }
  }
}