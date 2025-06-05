using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class BattleSystem : MonoBehaviour
{
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

    public void RegisterEnemy(EnemyBehavior enemy)
    {
        enemies.Add(enemy);
    }

    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyTurnRoutine()); 
    }

    private IEnumerator EnemyTurnRoutine()
    {
        Debug.Log($"There's currently {enemies.Count} enemies in the map");
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].Act();
            yield return new WaitForSeconds(0.05f); 
            enemies[i].SelectNextMove();
            yield return new WaitForSeconds(0.05f); 
        }
    }

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
