using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using System; 
using UnityEngine.U2D.Animation;
using Unity.VisualScripting;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private bool debugging; 

    [SerializeField]
    private float visibleRange, attackRange;

    [SerializeField]
    private CharacterMovement enemyMovement;

    [SerializeField]
    private LayerMask playerLayer;

    [SerializeField]
    private SpriteResolver spriteResolver;

    private EnemyStat enemyStat; 
    
    private string spriteResolverLabel = "Indicators";
    private string spriteName; 

    public Action nextAction;
    public void Start()
    {
        enemyStat = GetComponent<EnemyStat>();        
        BattleSystem.Instance.RegisterEnemy(this);
        spriteName = "None";
        UpdateActionIndicator();
    }
    public virtual void SelectNextMove()
    {
        spriteName = "None";
        
        // Check if the player is visible to the current character
        Collider2D hit = Physics2D.OverlapCircle(GetCurrentPosition(), visibleRange, playerLayer);
        if (hit != null)
        {
            // Note this might cause a bug if PlayerStats and CharacterMovement components are not on the same game object as the player collider
            if (hit.GetComponent<PlayerStats>() != null && hit.GetComponent<CharacterMovement>() != null)
            {                
                // Get player/target's position
                Vector3 targetLocation = hit.GetComponent<CharacterMovement>().movePoint.transform.position;

                float xDiff = targetLocation.x - GetCurrentPosition().x;
                float yDiff = targetLocation.y - GetCurrentPosition().y;

                if(debugging)
                    Debug.Log($"Current x and y diff from the player: ({xDiff},{yDiff})"); 

                // Going to attack player if in range
                if (xDiff == 0f && Mathf.Abs(yDiff) <= attackRange || yDiff == 0f && Mathf.Abs(xDiff) <= attackRange)
                {
                    Attack(new Vector2Int((int)targetLocation.x, (int)targetLocation.y), hit.gameObject); 
                }
                else
                {
                    Move(xDiff, yDiff, hit.gameObject);
                }
                UpdateActionIndicator();
            }
            else
            {
                if (debugging)
                    Debug.Log("Enemy detects something other than player"); 
            }
        }
        else
        {
            if (debugging)
                Debug.Log("Player is not visible to the enemy"); 
        }
    }
    private void UpdateActionIndicator()
    {
        spriteResolver.SetCategoryAndLabel(spriteResolverLabel, spriteName);
        spriteResolver.ResolveSpriteToSpriteRenderer();
    }

    // Execute the action
    public void Act()
    {
        // Checks if the next action is still reasonable after player's new action
        if(nextAction != null && CheckAction(nextAction.actionType))
        {
            if (nextAction.actionType == ActionType.MOVEMENT)
            {
                // TODO: change it so that the enemy wouldn't away if the character moved to another location that looks weird if the enemy moved to the origional planned location
                enemyMovement.Move(nextAction.movement.x, nextAction.movement.y);
                
                if (debugging)
                    Debug.Log($"Enemy moves by {nextAction.movement.x}, {nextAction.movement.y}");
            }
            else if (nextAction.actionType == ActionType.ATTACK)
            {
                // TODO: attack if the target is still in range
                BattleSystem.Instance.ExecuteAttack(enemyStat, nextAction.target.GetComponent<PlayerStats>()); 
            }
        }
        else
        {
            SelectNextMove();

            if (debugging)
                Debug.Log("Enemy chooses next move due to null nextAction"); 
        }
    }

    // Checks if the character's movement is still resonable
    private bool CheckAction(ActionType actionType)
    {
        switch (actionType) 
        {
            case ActionType.MOVEMENT:
                // Don't move if a character gets in the way
                
                // TODO: checks again by detect if currently there's anything there
                // because there's currently a bug where the two enemy will overlap

                Vector2 moveLocation = nextAction.movement + GetCurrentPosition();
                if (nextAction.target.GetComponent<CharacterMovement>().TruePosition() == moveLocation)
                {
                    if (debugging)
                        Debug.Log("Enemy decides to change movement because player gets in the way"); 

                    SelectNextMove();
                    return false; 
                }
                break; 

            case ActionType.ATTACK:
                // Don't attack if target character moved to a new location

                if (nextAction.targetLocation != nextAction.target.GetComponent<CharacterMovement>().TruePosition())
                {
                    if (debugging)
                        Debug.Log("Enemy decides to not attack because player is no long in the same position"); 

                    SelectNextMove();
                    return false; 
                }
                break; 
        }
        return true; 
    }

    // Decides where the character should move based on the target character's position
    // TODO: get a better path-finding
    // TODO: there's a bug where the character would still move up after player moved down
    private void Move(float xDiff, float yDiff, GameObject target)
    {
        nextAction = new Action(ActionType.MOVEMENT, target);
        // Move towards player
        if (Mathf.Abs(xDiff) > Mathf.Abs(yDiff))
        {
            nextAction.movement.x = (int)Mathf.Clamp(xDiff, -1, 1);
            if (xDiff > 0)
                spriteName = "Right";
            else if (xDiff < 0)
                spriteName = "Left";
        }
        else
        {
            nextAction.movement.y = (int)Mathf.Clamp(yDiff, -1, 1);
            if (yDiff > 0)
                spriteName = "Up";
            if (yDiff < 0)
                spriteName = "Down";
        }
    }

    // Decides to attack
    private void Attack(Vector2Int targetLocation, GameObject target)
    {
        nextAction = new Action(ActionType.ATTACK, target); 
        nextAction.targetLocation = targetLocation;
        spriteName = "Attack";
    }

    // Return current position of the MovePoint
    // The MovePoinet position is the "ture" position of this character
    private Vector2 GetCurrentPosition()
    {
        return new Vector2(enemyMovement.movePoint.transform.position.x, enemyMovement.movePoint.transform.position.y);
    }
}
