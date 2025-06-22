using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Singleton
public class InputSystem : MonoBehaviour
{
    [SerializeField] private CharacterMovement playerMovement;

    public UnityEvent OnMouseClick;
    public UnityEvent OnMouseRelease;
    public UnityEvent OnCharacterMove;

    public InputState inputState; 

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
        if (Input.GetKeyDown(Keys.pause)) { }

        switch (inputState)
        {
            case InputState.GAMEPLAY:
                MovementUpdate(); 
                break; 
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnMouseClick.Invoke();
        }
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(1))
        {
            OnMouseRelease.Invoke();
        }

    }

    private void MovementUpdate()
    {
        int x = 0;
        int y = 0;
        if (Input.GetKeyDown(Keys.left1) || Input.GetKeyDown(Keys.left2))
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
            BattleSystem.Instance.EndPlayerTurn();
            return;
        }
        if (x != 0 || y != 0)
        {
            playerMovement.Move(x, y);
            OnCharacterMove.Invoke();
        }
    }

    // Returns whether the player is currently dragging a card
    public bool IsDraggingACard()
    {
        return ActionCardController.Instance.GetDraggedCardIndex() != -1;
    }

    public void ChangeInputStateTo(InputState state)
    {
        inputState = state; 
    }

    public void StartPlayerTurn()
    {
        inputState = InputState.GAMEPLAY; 
    }
}

public enum InputState
{
    GAMEPLAY,
    WAIT_FOR_TURN,
    PAUSE
}
