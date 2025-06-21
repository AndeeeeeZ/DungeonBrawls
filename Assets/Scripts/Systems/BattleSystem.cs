using UnityEngine;
using System.Collections.Generic;
using System.Collections;

// Singleton
public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    private bool debugging;

    [SerializeField]
    private PlayerStats player; 
    public static BattleSystem Instance { get; private set; } 
    private List<EnemyStat> enemies;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;
        enemies = new List<EnemyStat>();
    }

    // Adds enemy into the list in the current scene
    public void RegisterEnemy(EnemyStat enemy)
    {
        enemies.Add(enemy);
    }

    // Removes enemy from the list in the current scene
    // Return a bool that indicates whether enemy is removed from the list
    public bool RemoveEnemy(EnemyStat enemy)
    {
        return enemies.Remove(enemy);
    }

    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurnRoutine()); 
    }

    // Go through the list of enemies in the scene 
    // Each enemy acts and select its next move
    private IEnumerator EnemyTurnRoutine()
    {
        if (debugging)
            Debug.Log($"There's currently {enemies.Count} enemies in the map");

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            enemies[i].enemyBehavior.Act();
            enemies[i].enemyBehavior.SelectNextMove();
            yield return new WaitForSeconds(0.1f); 
        }
    }

    // Returns the damage dealt
    // Damage is calculated by defender's defense - attacker's attack
    public int CalculateAttack(Character attacker, Character defender)
    {
        return(Mathf.Max(0, attacker.attack - defender.defense));
    }

    // Executes the attack
    // Return damage dealt to defender
    public int ExecuteAttack(Character attacker, Character defender)
    {
        int damage = CalculateAttack(attacker, defender);
        
        if (debugging)
            Debug.Log($"{attacker.name} made {damage} damage to {defender.name}");
        
        defender.ReduceHealthBy(damage);

        if (!defender.IsAlive())
        {
            if (debugging)
                Debug.Log($"{defender.name} died");
            defender.Die(); 
        }
        return damage; 
    }

    // TODO: When mouse is released, check which enemy is selected, if it's in range, etc.
    public void ExecuteActionCard()
    {
        EnemyStat target = GetActionCardTarget();
        if (target != null)
        {
            Debug.Log($"An action card is used on {target.gameObject.name}");
            ActionCardController.Instance.RemoveSelectedCard();

            ExecuteAttack(player, target); 
            
            StartEnemyTurn(); 
        }
    }

    private EnemyStat GetActionCardTarget()
    {
        foreach (EnemyStat enemy in enemies)
        {
            if (enemy.IsThisEnemySelected())
            {
                return enemy;
            }
        }
        return null; 
    }

    public Character GetCharacterAt(int x, int y)
    {
        Collider2D hit = Physics2D.OverlapPoint(new Vector3(x, y, 0f));
        if (hit != null)
        {
            if (hit.CompareTag("Character") && hit)
            {
                CharacterMovement characterMovement = hit.GetComponent<CharacterMovement>();
                if (characterMovement != null)
                {
                    if (characterMovement.TruePosition().x != x || characterMovement.TruePosition().y != y)
                    {
                        if (debugging)
                            Debug.Log($"Despite there seens to be a character at ({x},{y}), the true position of the character is not there");
                        return null; 
                    }
                }
                return hit.GetComponent<Character>();
            }
        }
        return null; 
    }
}
