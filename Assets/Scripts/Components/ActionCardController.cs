using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic; 

public class ActionCardController : MonoBehaviour
{
    [SerializeField]
    private bool debugging; 

    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private GraphicRaycaster raycaster;

    [SerializeField]
    private GameObject[] actionCardDisplays;

    [SerializeField] 
    private ActionCard[] actionCards;

    [SerializeField]
    private float gapBetweenCards, selectYOffset, cardMovementSpeed;

    [HideInInspector]
    public int maxCardLimit, currentCardAmount;

    private PointerEventData pointerEventData;

    private float[] targetXLocation; 
    private float[] targetYLocation;

    private void Start()
    {
        maxCardLimit = actionCardDisplays.Length;
        targetXLocation = new float[maxCardLimit];
        targetYLocation = new float[maxCardLimit];
    }


    private void Update()
    {
        // Move the action card to the location they are supposed to be
        for (int i = 0; i < actionCardDisplays.Length; i++)
        {
            Transform actionCardDisplayTransform = actionCardDisplays[i].transform;
            Vector3 target = new Vector3(actionCardDisplayTransform.localPosition.x,
                targetYLocation[i],
                actionCardDisplayTransform.localPosition.z);
            actionCardDisplays[i].transform.localPosition = Vector3.Lerp(actionCardDisplays[i].transform.localPosition, target, cardMovementSpeed * Time.deltaTime);
        }
    }

    // Checks if the player clicks on the action cards
    // Works by the Unity Event System
    public void CheckMouseClick()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition; 

        List<RaycastResult> results = new List<RaycastResult>();    
        raycaster.Raycast(pointerEventData, results);

        foreach(RaycastResult result in results)
        {
            if (debugging)
            {
                Debug.Log("Mouse clicked on something"); 
            }

            // Check the index of the selected action card
            for (int i = 0; i < actionCardDisplays.Length; i++)
            {
                if (actionCardDisplays[i] == result.gameObject)
                {
                    if (debugging)
                    {
                        Debug.Log($"The index of the selected card is {i}");
                    }
                    SelectCard(i); 
                }
            }
        }
    }

    // Move up the selected card
    private void SelectCard(int index)
    {
        for (int i = 0; i < actionCardDisplays.Length; i++)
        {
            targetYLocation[i] = 0f; 
            if (i == index)
            {
                targetYLocation[index] = selectYOffset;
            }
        }
    }

    

    // Change current card amount by n
    private void ChangeCurrentCardAmountBy(int n)
    {
        currentCardAmount += n; 
        if (currentCardAmount <= 0 ||  currentCardAmount > maxCardLimit)
        {
            Debug.LogWarning("Card amount exceeds max card amount"); 
        }
        currentCardAmount = Mathf.Clamp(currentCardAmount, 1, maxCardLimit);
        CalculateTargetLocation(); 
    }

    // Calculates the x position the cards need to move to 
    // In order to maintain the gap while centering the group of cards
    private void CalculateTargetLocation()
    {
        // TODO
    }
}
