using UnityEngine.UI; 
using UnityEngine;

public class ActionCardContainer : MonoBehaviour
{
    //[HideInInspector]
    public GameObject actionCardDisplay;

    //[HideInInspector]
    public ActionCard actionCardScriptableObject;

    
    public float targetXLocation, targetYLocation, 
                 targetXScale, targetYScale;

    [SerializeField]
    private float moveUpSpeed, moveDownSpeed;

    public void ReceiveInfo(GameObject actionCardUIPrefab, GameObject parentObject, float spawnX, float spawnY, ActionCard actionCardObject)
    {
        actionCardDisplay = Instantiate(actionCardUIPrefab, parentObject.transform);
        actionCardDisplay.transform.localPosition = new Vector3(spawnX, spawnY, 0f);
        actionCardScriptableObject = actionCardObject;
    }

    private void Start()
    {
        moveUpSpeed = ActionCardController.Instance.cardMoveUpSpeed;
        moveDownSpeed = ActionCardController.Instance.cardMoveDownSpeed; 
    }

    public void UpdateCardDisplay()
    {
        Vector3 target = new Vector3(
                    targetXLocation,
                    targetYLocation,
                    actionCardDisplay.transform.localPosition.z);
        
        if (actionCardDisplay.transform.localPosition.y > target.y)
        {
            actionCardDisplay.transform.localPosition = Vector3.Lerp(actionCardDisplay.transform.localPosition, target, moveUpSpeed * Time.deltaTime); 
        }
        else
        {
            actionCardDisplay.transform.localPosition = Vector3.Lerp(actionCardDisplay.transform.localPosition, target, moveDownSpeed * Time.deltaTime);
        }
    }

    public void UpdateSprite() 
    {
        actionCardDisplay.GetComponent<Image>().sprite = actionCardScriptableObject.sprite; 
    }

    public void DestorySelf()
    {
        Destroy(actionCardDisplay);
        Destroy(this); 
    }

}
