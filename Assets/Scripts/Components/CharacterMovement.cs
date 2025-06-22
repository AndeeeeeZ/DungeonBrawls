using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float stepSize, moveSpeed;

    [SerializeField] 
    private SpriteController spriteController; 

    // MovePoint is the point that the character is going to move towards
    // MovePoint is used as the "true" position of the character
    public Transform movePoint;

    // The layer that characters cannot move through
    [SerializeField]
    private LayerMask obstacleLayer;

    public UnityEvent PlayerArrivedTargetLocation;


    // A flag use to prevent detection for arriving at target location to be triggered multiple times
    private bool moving; 

    private void Start()
    {
        // Prevents child moving the parent
        movePoint.parent = null; 
        movePoint.position = transform.position;

        moving = false; 

        if (movePoint == null)
        {
            Debug.LogWarning("Movement point missing");
        }
    }

    private void Update()
    {
        if (transform.position != movePoint.position && moving)
            MoveTowardTarget(); 
    }

    // Moves character towards MovePoint
    private void MoveTowardTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
        if (transform.position == movePoint.position && moving)
        {
            moving = false; 
            PlayerArrivedTargetLocation.Invoke();
            spriteController?.StopWalking();
            
            // TODO: FIX THIS, THIS IS HORRIBLE DESIGN
            if (gameObject.GetComponent<PlayerStats>() != null)
                BattleSystem.Instance.EndPlayerTurn(); 
            else 
                BattleSystem.Instance.EnterNextTurn();
        }
    }

    // Move MovePoint
    public void Move(int x, int y)
    {
        Mathf.Clamp(x, -1, 1);
        Mathf.Clamp(y, -1, 1);

        if (x == 0 && y == 0)
            return;

        Vector3 moveLocation = movePoint.position + new Vector3(x * stepSize, y * stepSize, 0f);

        // Checks if there is any character at the target location
        Collider2D hit = Physics2D.OverlapPoint(moveLocation);

        // TODO: remove/fix this
        if (hit != null)
        {
            Debug.LogWarning("Character didn't move because something is in the way"); 
            if (hit.CompareTag("Character") && hit)
            {

            }
        }
        // Move target point if it's not going into an obstacle
        else if (!Physics2D.OverlapCircle(moveLocation, 0f, obstacleLayer))
        {
            movePoint.Translate(new Vector2(x * stepSize, y * stepSize));
            spriteController?.StartWalking(x, y);
            moving = true;
            InputSystem.Instance.ChangeInputStateTo(InputState.WAIT_FOR_TURN);
        }
    }  

    public Vector2Int TruePosition()
    {
        return new Vector2Int((int)movePoint.transform.position.x, (int)movePoint.transform.position.y);
    }
}
