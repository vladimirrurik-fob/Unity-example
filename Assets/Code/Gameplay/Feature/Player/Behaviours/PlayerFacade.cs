using System;
using UnityEngine;
using VContainer;

namespace Code.Gameplay.Feature.Player.Behaviours
{
  public class PlayerFacade : MonoBehaviour
  {
    [SerializeField] private PlayerHealth _playerHealth;
    private IPlayerService _playerService;

    public PlayerHealth PlayerHealth => _playerHealth;

    [Inject]
    public void Construct(IPlayerService playerService)
    {
      _playerService = playerService;
      _playerService.Register(this);
    }
  }
}