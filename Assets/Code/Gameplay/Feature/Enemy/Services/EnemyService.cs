using System.Collections.Generic;
using UnityEngine;

namespace Code.Gameplay.Feature.Enemy.Services
{
  public class EnemyService : MonoBehaviour
  {
    private List<EnemyHealth> _enemyHealthList = new();
    
    public void AddEnemy(EnemyHealth enemyHealth)
    {
      _enemyHealthList.Add(enemyHealth);
    }

    public void DestroyAllEnemy()
    {
      for (int i = 0; i < _enemyHealthList.Count; i++) 
        _enemyHealthList[i].Destroy();
    }
  }
}