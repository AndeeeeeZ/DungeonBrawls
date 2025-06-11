using UnityEngine;
using UnityEngine.Android;

public class SpriteController : MonoBehaviour
{
    private Animator animator;
    private int x, y; 

    private void Start()
    {
        animator = GetComponent<Animator>();
        x = 0;
        y = -1; 
    }

    public void StartWalking(int x, int y)
    {            
        animator.SetTrigger("START");
        if (this.x != x || this.y != y)
        {
            this.x = x;
            this.y = y;
            animator.SetInteger("X", this.x); 
            animator.SetInteger("Y", this.y);
        }
    }

    public void StopWalking()
    {
        animator.SetTrigger("STOP"); 
    }

}
