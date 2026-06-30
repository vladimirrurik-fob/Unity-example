using Code.Core.Abstractions;
using Code.Core.Data;
using Code.Infrastructure.Services.Analytic;
using TMPro;
using UnityEngine;
using VContainer;

namespace Code.Gameplay.Feature.Player.Behaviours
{
  // Owns the player's health and is the ISaveLoad adapter that persists it into
  // the shared PlayerProgress (and rehydrates it on load).
  public class PlayerHealth : MonoBehaviour, ISaveLoad
  {
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth = 100;
    [SerializeField] private TextMeshProUGUI _healthText;

    private IAnalyticService _analyticService;

    [Inject]
    public void Construct(IAnalyticService analyticService)
    {
      _analyticService = analyticService;
      Debug.Log("Player Health Inject");
    }

    private void Awake()
    {
      Debug.Log("Player Health Awake");
      RefreshText();
    }

    private void Start() =>
      Debug.Log("Player Health Start");

    public void TakeDamage(int damage)
    {
      _currentHealth = Mathf.Max(0, _currentHealth - damage);
      RefreshText();
      _analyticService?.Log("PlayerTakeDamage");
    }

    public int GetCurrentHealth() =>
      _currentHealth;

    public void SetCurrentHealth(int health)
    {
      _currentHealth = health;
      RefreshText();
    }

    private void RefreshText()
    {
      if (_healthText != null)
      {
        _healthText.text = $"HP: {_currentHealth}";
      }
    }

    // --- ISaveLoad: persist / restore the player's health ---
    public void Save(PlayerProgress progress)
    {
      progress.PlayerData.CurrentHealth = _currentHealth;
    }

    public void Load(PlayerProgress progress)
    {
      _currentHealth = progress.PlayerData.CurrentHealth;
      RefreshText();
      Debug.Log($"[SaveLoad] PlayerHealth restored to {_currentHealth}");
    }

    public void Reset(PlayerProgress progress)
    {
      _currentHealth = _maxHealth;
      RefreshText();
    }
  }
}
