using UnityEngine;

public class Action
{
    public Vector2Int movement;
    public Vector2Int targetLocation; 
    public ActionType actionType;
    public Character target; 
    public Action(ActionType actionType)
    {
        this.actionType = actionType; 
    }

    public Action(ActionType actionType, Character target)
    {
        this.actionType = actionType;
        this.target = target;
    }
}

public enum ActionType
{
    MOVEMENT, ATTACK
}
