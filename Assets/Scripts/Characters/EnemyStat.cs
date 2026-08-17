using UnityEngine;
using UnityEngine.Rendering;

public class EnemyStat: Character
{
    [SerializeField]
    private GameObject selectionIndicator;

    [HideInInspector]
    public EnemyBehavior enemyBehavior;

    private BoxCollider2D boxCollider;

    private bool registered; 
    
    private void Start()
    {
        registered = false; 
        enemyBehavior = GetComponent<EnemyBehavior>();
        boxCollider = GetComponent<BoxCollider2D>();

        HideSelectionIndicator();

        if (enemyBehavior == null)
        {
            Debug.LogWarning($"{this.gameObject.name} is missing enemyBehavior");
        }
    }

    private void Update()
    {
        // Had to do this instead of putting this in start because there's a weird bug caused by the execution order
        if (!registered)
        {
            BattleSystem.Instance.RegisterEnemy(this);
        }
        UpdateSelectionIndicator();
    }

    // Shows the selection indicator if the player is dragging a card and currently over the enemy
    private void UpdateSelectionIndicator()
    {
        if (IsThisEnemySelected())
            ShowSelectionIndicator();
        else 
            HideSelectionIndicator();
    }

    // Return the reference to the current EnemyStat object if mouse is 
    public EnemyStat GetSelectedEnemy()
    {
        if (InputSystem.Instance.IsDraggingACard())
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
            if (hit.collider == boxCollider)
                return this;
            else
                return null;
        }
        return null; 
    }

    // Return a bool for whether an enemy is currently selected
    public bool IsThisEnemySelected()
    {
        return GetSelectedEnemy() != null; 
    }
    public override void Die()
    {
        if (!BattleSystem.Instance.RemoveEnemy(this))
        {
            Debug.LogWarning($"{this.gameObject.name} failed to be removed from the Battle System list after death");
        }    
        this.gameObject.SetActive(false);
    }

    private void ShowSelectionIndicator()
    {
        selectionIndicator.SetActive(true);
    }

    private void HideSelectionIndicator()
    {
        selectionIndicator.SetActive(false);
    }
}
