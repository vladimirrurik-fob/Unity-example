using UnityEngine;

namespace Code.Gameplay.Feature.Enemy
{
  public interface IEnemyJump
  {
    void Jump();
  }
  
  public abstract class EnemyBase : MonoBehaviour
  {
    public abstract void Attack();
    // public abstract void Jump();
  }

  public class MeleeEnemy : EnemyBase, IEnemyJump
  {
    public override void Attack()
    {
      //Attack
    }

    public void Jump()
    {
      //Jump
    }
  }

  public class FlyEnemy : EnemyBase
  {
    public override void Attack()
    {
      //Attack
    }
  }
}