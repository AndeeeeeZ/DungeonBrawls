using UnityEngine;
using System; 
using UnityEngine.U2D.Animation;

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
    private PlayerStats targetPlayer; 
    
    private string spriteResolverLabel = "Indicators";
    private string spriteName; 

    public Action nextAction;
    public void Start()
    {
        enemyStat = GetComponent<EnemyStat>();
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
                if(targetPlayer == null) 
                    targetPlayer = hit.GetComponent<PlayerStats>(); 

                // Get player/target's position
                Vector3 targetLocation = hit.GetComponent<CharacterMovement>().movePoint.transform.position;

                float xDiff = targetLocation.x - GetCurrentPosition().x;
                float yDiff = targetLocation.y - GetCurrentPosition().y;

                if(debugging)
                    Debug.Log($"Current x and y diff from the player: ({xDiff},{yDiff})"); 

                // Going to attack player if in range
                if (xDiff == 0f && Mathf.Abs(yDiff) <= attackRange || yDiff == 0f && Mathf.Abs(xDiff) <= attackRange)
                {
                    Attack(new Vector2Int((int)targetLocation.x, (int)targetLocation.y)); 
                }
                else
                {
                    Move(xDiff, yDiff);
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
        // TODO: Clean up this code structure
        // Checks if the next action is still reasonable after player's new action
        if(nextAction != null)
        {
            if (nextAction.actionType == ActionType.MOVEMENT)
            {
                if (BattleSystem.Instance.GetCharacterAt((int)transform.position.x + nextAction.targetMovementDirection.x, 
                                                         (int)transform.position.y + nextAction.targetMovementDirection.y) == null)
                { 
                    enemyMovement.Move(nextAction.targetMovementDirection.x, nextAction.targetMovementDirection.y);
                    if (debugging)
                        Debug.Log($"Enemy moves by {nextAction.targetMovementDirection.x}, {nextAction.targetMovementDirection.y}");
                }
                else
                {
                    if (debugging)
                        Debug.Log("Enemy decides to change movement because player gets in the way");
                    BattleSystem.Instance.EnterNextTurn();
                }
                // TODO: change it so that the enemy wouldn't away if the character moved to another location that looks weird if the enemy moved to the origional planned location   
            }
            else if (nextAction.actionType == ActionType.ATTACK)
            {
                Character target = BattleSystem.Instance.GetCharacterAt(nextAction.targetAttackLocation.x, nextAction.targetAttackLocation.y);
                if (target != null)
                {
                    BattleSystem.Instance.ExecuteAttack(enemyStat, target);
                }
                else
                {
                    if (debugging)
                        Debug.Log("Enemy attack missed");
                }        
                BattleSystem.Instance.EnterNextTurn();
            }
        }
        else
        {
            // If character didn't have a nextAction
            BattleSystem.Instance.EnterNextTurn();
        }
            SelectNextMove();

    }

    // Decides where the character should move based on the target character's position
    // TODO: get a better path-finding
    // TODO: there's a bug where the character would still move up after player moved down
    private void Move(float xDiff, float yDiff)
    {
        nextAction = new Action(ActionType.MOVEMENT);
        // Move towards player
        if (Mathf.Abs(xDiff) > Mathf.Abs(yDiff))
        {
            nextAction.targetMovementDirection.x = (int)Mathf.Clamp(xDiff, -1, 1);
            if (xDiff > 0)
                spriteName = "Right";
            else if (xDiff < 0)
                spriteName = "Left";
        }
        else
        {
            nextAction.targetMovementDirection.y = (int)Mathf.Clamp(yDiff, -1, 1);
            if (yDiff > 0)
                spriteName = "Up";
            if (yDiff < 0)
                spriteName = "Down";
        }
    }

    // Decides to attack
    private void Attack(Vector2Int targetAttackLocation)
    {
        nextAction = new Action(ActionType.ATTACK); 
        nextAction.targetAttackLocation = targetAttackLocation;
        spriteName = "Attack";
    }

    // Return current position of the MovePoint
    // The MovePoinet position is the "ture" position of this character
    private Vector2 GetCurrentPosition()
    {
        return new Vector2(enemyMovement.movePoint.transform.position.x, enemyMovement.movePoint.transform.position.y);
    }
}
