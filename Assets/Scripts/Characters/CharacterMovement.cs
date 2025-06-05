using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField]
    private float stepSize, moveSpeed;

    // MovePoint is the point that the character is going to move towards
    // MovePoint is used as the "true" position of the character
    public Transform movePoint;

    // The layer that characters cannot move through
    [SerializeField]
    private LayerMask obstacleLayer;

    private Character character; 

    private void Start()
    {
        // Prevents child moving the parent
        movePoint.parent = null; 
        movePoint.position = transform.position;
        character = GetComponent<Character>();
        
        if (character == null)
        {
            Debug.LogWarning("Character missing"); 
        }
        if (movePoint == null)
        {
            Debug.LogWarning("Movement point missing");
        }
    }

    private void Update()
    {
        // Moves character towards MovePoint
        if (transform.position != movePoint.position) 
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
    }

    // Move MovePoint
    public void Move(int x, int y)
    {
        Mathf.Clamp(x, -1, 1);
        Mathf.Clamp(y, -1, 1);

        if (x == 0 && y == 0)
            return; 

        Vector3 moveLocation = movePoint.position + new Vector3(x, y, movePoint.position.z);

        // Checks if there is any character at the target location
        Collider2D hit = Physics2D.OverlapPoint(moveLocation);

        // TODO: moving this to a different location / new class
        if (hit != null)
        {
            if (hit.CompareTag("Character") && hit)
            {
                Character defender = hit.GetComponent<Character>();
                if (defender != null)
                {
                    BattleSystem.Instance.ExecuteAttack(character, defender);
                }
                else
                {
                    Debug.Log("Character component not detected");
                }
            }
        }
        // Move target point if it's not going into an obstacle
        else if (!Physics2D.OverlapCircle(moveLocation, 0f, obstacleLayer))
        {
            movePoint.Translate(new Vector2(x * stepSize, y * stepSize));
        }

    }  

    public Vector2Int TruePosition()
    {
        return new Vector2Int((int)movePoint.transform.position.x, (int)movePoint.transform.position.y);
    }
}
