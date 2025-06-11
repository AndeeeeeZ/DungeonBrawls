using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private Animator animator;
    private int x, y;
    private bool isMoving; 

    private void Start()
    {
        animator = GetComponent<Animator>();
        x = 0;
        y = 0;
        isMoving = false; 
        UpdateAnimator();
    }

    public void StartWalking(int x, int y)
    {            
        // Triggers if in idle state or changed direction
        if (this.x != x || this.y != y || !isMoving)
        {
            animator.SetTrigger("START");
            isMoving = true; 
            this.x = x;
            this.y = y;
            UpdateAnimator(); 
        }
    }

    private void UpdateAnimator()
    {
        animator.SetInteger("X", this.x);
        animator.SetInteger("Y", this.y);
    }

    // Stops walking animaiton
    public void StopWalking()
    {
        animator.SetTrigger("STOP");
        isMoving = false; 
    }

}
