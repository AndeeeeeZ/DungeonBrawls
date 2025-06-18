using UnityEngine;

public class EnemyStat: Character
{
    private EnemyBehavior enemyBehavior;

    private void Start()
    {
        enemyBehavior = GetComponent<EnemyBehavior>();
        if (enemyBehavior == null)
        {
            Debug.LogWarning($"{this.gameObject.name} is missing enemyBehavior");
        }
    }
    public override void Die()
    {
        if (!BattleSystem.Instance.RemoveEnemy(enemyBehavior))
        {
            Debug.LogWarning($"{this.gameObject.name} failed to be removed from the Battle System list after death");
        }
        
        this.gameObject.SetActive(false);
    }
}
