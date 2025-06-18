using UnityEngine;
using UnityEngine.Events;

// Singleton
public class InputSystem : MonoBehaviour
{
    [SerializeField] private CharacterMovement playerMovement;
    [SerializeField] private SpriteController spriteController;

    public UnityEvent OnMouseClick;
    public UnityEvent OnMouseRelease;
    public UnityEvent OnCharacterMove;

    public static InputSystem Instance { get; private set; }

    private void Start()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }   
    private void Update()
    {
        int x = 0;
        int y = 0; 
        if (Input.GetKeyDown(Keys.left1) ||  Input.GetKeyDown(Keys.left2))
        {
            x--; 
        }
        else if (Input.GetKeyDown(Keys.right1) || Input.GetKeyDown(Keys.right2))
        {
            x++;
        }
        else if (Input.GetKeyDown(Keys.up1) || Input.GetKeyDown(Keys.up2))
        {
            y++; 
        }
        else if (Input.GetKeyDown(Keys.down1) || Input.GetKeyDown(Keys.down2))
        {
            y--; 
        }
        if (Input.GetKeyDown(Keys.skip))
        {
            BattleSystem.Instance.StartEnemyTurn();
            return;
        }
        if (Input.GetKeyDown(Keys.pause))
        {
            
        }
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseClick.Invoke();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnMouseRelease.Invoke();
        }
        if (x != 0 || y != 0) 
        {
            playerMovement.Move(x, y);
            spriteController.StartWalking(x, y);
            BattleSystem.Instance.StartEnemyTurn(); 
            OnCharacterMove.Invoke();
        }   
    }

    // Returns whether the player is currently dragging a card
    public bool IsDraggingACard()
    {
        return ActionCardController.Instance.GetDraggedCardIndex() != -1;
    }
}
