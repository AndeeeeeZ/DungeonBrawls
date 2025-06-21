using UnityEngine;

public class Action
{
    // Local Position
    public Vector2Int targetMovementDirection;
    // Global Position
    public Vector2Int targetAttackLocation; 
    public ActionType actionType;
    // public GameObject target; 
    public Action(ActionType actionType)
    {
        this.actionType = actionType; 
    }

    public Action(ActionType actionType, Vector2Int targetAttackLocation)
    {
        this.actionType = actionType;
        this.targetAttackLocation = targetAttackLocation;
    }
}

public enum ActionType
{
    MOVEMENT, ATTACK
}
