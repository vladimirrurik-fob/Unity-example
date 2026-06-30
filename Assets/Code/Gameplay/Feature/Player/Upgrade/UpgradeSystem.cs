using Code.Gameplay.Feature.Player.Behaviours;
using UnityEngine;

namespace Code.Gameplay.Feature.Player.Upgrade
{
  public class UpgradeSystem 
  {
    private IPlayerService _playerService;

    public UpgradeSystem(IPlayerService playerService)
    {
      
    }
    
    public void Construct(IPlayerService playerService)
    {
      _playerService = playerService;
    }
    
    public void Upgrade()
    {
      // _playerService.PlayerFacade.PlayerHealth.UpgradeHealth();
    }
  }
}