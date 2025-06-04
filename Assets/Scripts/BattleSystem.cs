using UnityEngine;

public static class BattleSystem 
{
    public static int CalculateAttack(Character attacker, Character defender)
    {
        return(Mathf.Max(0, attacker.attack - defender.defense));
    }

    // Executes the attack
    // Return damage dealt to defender
    public static int ExecuteAttack(Character attacker, Character defender)
    {
        int damage = BattleSystem.CalculateAttack(attacker, defender);
        Debug.Log($"{damage} is made");
        defender.ReduceHealthBy(damage);
        return damage; 
    }
}
