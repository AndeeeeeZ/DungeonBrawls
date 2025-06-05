using UnityEngine;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private CharacterMovement playerMovement; 
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

        }
        if (Input.GetKeyDown(Keys.pause))
        {

        }
        if (x != 0 || y != 0) 
        {
            playerMovement.Move(x, y);
            BattleSystem.Instance.StartEnemyTurn(); 
        }   
    }
}
