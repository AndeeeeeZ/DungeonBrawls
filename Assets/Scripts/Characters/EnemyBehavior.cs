using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using System; 
using UnityEngine.U2D.Animation;
using Unity.VisualScripting;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField]
    private float visibleRange, attackRange;

    [SerializeField]
    private CharacterMovement enemyMovement;

    [SerializeField]
    private LayerMask playerLayer;

    [SerializeField]
    private SpriteResolver spriteResolver;

    private Enemy enemyStats; 
    
    private string spriteResolverLabel = "Indicators";
    private string spriteName; 

    public Action nextAction;

    private Vector3 currPosition; 

    public void Start()
    {
        // Not using transform.position because the character need to be at the new location when making the next move
        currPosition = transform.position;
        enemyStats = GetComponent<Enemy>(); 
    }
    public virtual void SelectNextMove()
    {
        spriteName = "";
        
        Collider2D hit = Physics2D.OverlapCircle(currPosition, visibleRange, playerLayer);
        if (hit != null)
        {
            if (hit.GetComponent<PlayerStats>() != null)
            {
                Vector3 targetLocation = hit.transform.position;
                float xDiff = targetLocation.x - currPosition.x;
                float yDiff = targetLocation.y - currPosition.y;
                Debug.Log($"Current x and y diff: ({xDiff},{yDiff})"); 
                // Attack player if in range
                if (xDiff == 0f && Mathf.Abs(yDiff) <= attackRange || yDiff == 0f && Mathf.Abs(xDiff) <= attackRange)
                {
                    Attack(new Vector2Int((int)targetLocation.x, (int)targetLocation.y), hit.GetComponent<PlayerStats>()); 
                }
                else
                {
                    Move(xDiff, yDiff);
                }
                spriteResolver.SetCategoryAndLabel(spriteResolverLabel, spriteName);
                spriteResolver.ResolveSpriteToSpriteRenderer(); 
            }
        }
        else
        {
            Debug.Log("Player not detected in scene"); 
        }
    }

    public void Act()
    {
        if(nextAction != null)
        {
            if (nextAction.actionType == ActionType.MOVEMENT)
            {
                // TODO: change it so that the enemy wouldn't away if the character moved to another location that looks weird if the enemy moved to the origional planned location
                enemyMovement.Move(nextAction.movement.x, nextAction.movement.y);
                currPosition += new Vector3(nextAction.movement.x, nextAction.movement.y, 0f);
                Debug.Log($"Enemy moves by {nextAction.movement.x}, {nextAction.movement.y}");
            }
            else if (nextAction.actionType == ActionType.ATTACK)
            {
                // TODO: attack if the target is still in range
                BattleSystem.ExecuteAttack(enemyStats, nextAction.target); 
            }
        }
        SelectNextMove();
    }

    private void Move(float xDiff, float yDiff)
    {
        nextAction = new Action(ActionType.MOVEMENT);
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

    // Act out the attack
    private void Attack(Vector2Int targetLocation, Character target)
    {
        nextAction = new Action(ActionType.ATTACK, target); 
        nextAction.targetLocation = targetLocation;
        spriteName = "Attack";
        // Debug.Log("Enemy is going to attack");
    }
}
