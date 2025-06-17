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
        currentCardAmount = actionCards.Length; 
        targetXLocation = new float[maxCardLimit];
        targetYLocation = new float[maxCardLimit];
        CalculateTargetLocation();
        UpdateDisplaySprite(); 
    }

    private void Update()
    {
        CalculateTargetLocation();
        // Move the action card to the location they are supposed to be
        for (int i = 0; i < actionCardDisplays.Length; i++)
        {
            Transform actionCardDisplayTransform = actionCardDisplays[i].transform;
            Vector3 target = new Vector3(
                targetXLocation[i],
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
    public void ChangeCurrentCardAmountBy(int n)
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
        UpdateDisplayActivity(); 
        float totalDistance = (currentCardAmount - 1) * gapBetweenCards;
        targetXLocation[0] = -(totalDistance / 2f); 
        for (int i = 1; i < currentCardAmount; i++)
        {
            targetXLocation[i] = targetXLocation[i-1] + gapBetweenCards;    
        }

        // New cards spawns in the location of the right most card
        for (int i = currentCardAmount; i < maxCardLimit; i++)
        {
            targetXLocation[i] = targetXLocation[currentCardAmount - 1]; 
        }
    }

    // Update all the current cards UI as active, vice versa
    private void UpdateDisplayActivity()
    {
        for (int i = 0; i < maxCardLimit; i++)
        {
            if (i >= currentCardAmount)
                actionCardDisplays[i].SetActive(false);
            else
                actionCardDisplays[i].SetActive(true);
        }
    }

    private void UpdateDisplaySprite()
    {
        for (int i = 0; i < actionCards.Length; i++)
        {
            actionCardDisplays[i].GetComponent<Image>().sprite = actionCards[i].sprite;
        }
    }
}
