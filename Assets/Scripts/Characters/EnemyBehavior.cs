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

    // SpriteResolver for action indicator
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
                    DecideToAttack(new Vector2Int((int)targetLocation.x, (int)targetLocation.y)); 
                }
                else
                {
                    DecideToMove(xDiff, yDiff);
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
        if (nextAction == null)
        {
            BattleSystem.Instance.EnterNextTurn();
            SelectNextMove();
            return;
        }

        switch (nextAction.actionType) 
        {
            case ActionType.MOVEMENT:
                HandleMovementAction(); 
                break; 

            case ActionType.ATTACK:
                HandleAttackAction();
                break; 
        }
        SelectNextMove();
    }

    // Execute the attack action
    private void HandleAttackAction()
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

    // Execute the movement action
    private void HandleMovementAction()
    {
        if (BattleSystem.Instance.GetCharacterAt((int)transform.position.x + nextAction.targetMovementDirection.x,
                                         (int)transform.position.y + nextAction.targetMovementDirection.y) == null)
        {
            // No enter next turn here because it's called when character arrives at target location
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
    }

    // Decides where the character should move based on the target character's position
    // TODO: get a better path-finding
    // TODO: there's a bug where the character would still move up after player moved down
    private void DecideToMove(float xDiff, float yDiff)
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
    private void DecideToAttack(Vector2Int targetAttackLocation)
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
