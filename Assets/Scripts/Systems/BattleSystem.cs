using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    private bool debugging; 
    public static BattleSystem Instance { get; private set; } 
    private List<EnemyBehavior> enemies;

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; 
        }
        Instance = this;
        enemies = new List<EnemyBehavior>();
    }

    // Adds enemy into the list in the current scene
    public void RegisterEnemy(EnemyBehavior enemy)
    {
        enemies.Add(enemy);
    }

    // Removes enemy from the list in the current scene
    public void RemoveEnemy(EnemyBehavior enemy)
    {
        enemies.Remove(enemy);
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
            enemies[i].Act();
            enemies[i].SelectNextMove();
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
        Debug.Log($"{damage} damage is made");
        defender.ReduceHealthBy(damage);
        return damage; 
    }
}
