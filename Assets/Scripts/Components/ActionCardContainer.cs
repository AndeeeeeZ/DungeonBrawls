using UnityEngine.UI; 
using UnityEngine;

public class ActionCardContainer : MonoBehaviour
{
    //[HideInInspector]
    public GameObject actionCardDisplay;

    //[HideInInspector]
    public ActionCard actionCardScriptableObject;

    public float targetXLocation, targetYLocation, targetScale, targetRotation;

    private float moveUpSpeed, moveDownSpeed, scaleUpSpeed, scaleDownSpeed;

    // A fake constructor since MonoBehaviour class can't be created with the "new" keyword
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
        scaleUpSpeed = ActionCardController.Instance.cardScaleUpSpeed;
        scaleDownSpeed = ActionCardController.Instance.cardScaleDownSpeed;
    }

    public void UpdateCardDisplay()
    {
        Vector3 transformTarget = new Vector3(
                    targetXLocation,
                    targetYLocation,
                    actionCardDisplay.transform.localPosition.z);

        Vector3 scaleTarget = new Vector3(targetScale, targetScale, targetScale);

        actionCardDisplay.transform.localRotation = Quaternion.Euler(0f, 0f, targetRotation); 

        // Scale at different speed depending on scaling up or down
        if (actionCardDisplay.transform.localScale.x < targetScale)
        {
            actionCardDisplay.transform.localScale = Vector3.Lerp(actionCardDisplay.transform.localScale, scaleTarget, scaleUpSpeed * Time.deltaTime);
        }
        else
        {
            actionCardDisplay.transform.localScale = Vector3.Lerp(actionCardDisplay.transform.localScale, scaleTarget, scaleDownSpeed * Time.deltaTime);
        }

        // Moves at different speed depending on moving up or down
        if (actionCardDisplay.transform.localPosition.y > transformTarget.y)
        {
            actionCardDisplay.transform.localPosition 
                = Vector3.Lerp(actionCardDisplay.transform.localPosition, transformTarget, moveUpSpeed * Time.deltaTime); 
        }
        else
        {
            actionCardDisplay.transform.localPosition 
                = Vector3.Lerp(actionCardDisplay.transform.localPosition, transformTarget, moveDownSpeed * Time.deltaTime);
        }
    }

    // Update the action card display to match that of the action card scriptable object
    public void UpdateSprite() 
    {
        actionCardDisplay.GetComponent<Image>().sprite = actionCardScriptableObject.sprite; 
    }

    // Remove self and the action card display
    // TODO: Add the animation for moving to the discard pile here
    public void DestorySelf()
    {
        Destroy(actionCardDisplay);
        Destroy(this); 
    }

}
