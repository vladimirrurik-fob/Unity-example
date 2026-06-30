namespace Code.Gameplay.Examples
{
  public interface IBadEnemyInterface
  {
    void TakeDamage(int damage);
    void Die();
    void Initialize();
    void PlayDieAnimation();
  }
}