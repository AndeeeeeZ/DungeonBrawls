using UnityEngine;
using UnityEngine.Events;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private CharacterMovement playerMovement;
    [SerializeField] private SpriteController spriteController;

    public UnityEvent OnMouseClick; 

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
        //if (Input.GetMouseButtonDown(0))
        //{
        //    OnMouseClick.Invoke();
        //}
        if (x != 0 || y != 0) 
        {
            playerMovement.Move(x, y);
            spriteController.StartWalking(x, y);
            BattleSystem.Instance.StartEnemyTurn(); 
        }   
    }
}
