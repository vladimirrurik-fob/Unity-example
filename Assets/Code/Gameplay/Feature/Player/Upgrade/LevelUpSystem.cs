using System;
using Code.Gameplay.Feature.Player.Behaviours;
using UnityEngine;

namespace Code.Gameplay.Feature.Player.Upgrade
{
  public class LevelUpSystem : MonoBehaviour
  {
    [SerializeField] private PlayerHealth _playerHealth;

    private void Start()
    {
      // _playerHealth.OnExpChange += LevelUp;
    }

    public void LevelUp(int exp)
    {
      
    }
  }
}