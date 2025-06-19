using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using JetBrains.Annotations;

// Singleton
public class ActionCardController : MonoBehaviour
{
    [SerializeField]
    private bool debugging;

    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private GraphicRaycaster raycaster;

    [SerializeField]
    private GameObject actionCardUIPrefab;

    [SerializeField]
    private ActionCard[] actionCardScriptableObjects; 

    [SerializeField]
    private float gapBetweenCards, selectYOffset, cardMoveUpSpeed, cardMoveDownSpeed;

    [HideInInspector]
    public int currentCardAmount;

    public int maxCardAmount; 

    public static ActionCardController Instance { get; private set; }

    private PointerEventData pointerEventData;

    private List<GameObject> actionCardDisplays;
    private List<ActionCard> actionCards;
    private List<float> targetXLocation; 
    private List<float> targetYLocation;

    private int draggedCardIndex; 

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        targetXLocation = new List<float>();
        targetYLocation = new List<float>();
        actionCardDisplays = new List<GameObject>(); 
        actionCards = new List<ActionCard>();

        currentCardAmount = 0; 

        InitializeActionCardList(3); 

        draggedCardIndex = -1; 

        CalculateTargetLocation();
    }

    // Initialize with n action cards
    private void InitializeActionCardList(int n)
    {
        for (int i = 0; i < n; i++)
        {
            AddCard(i, 0); 
        }
    }

    public void AddCard(int cardObjectIndex)
    {
        AddCard(actionCardDisplays.Count, cardObjectIndex);
    }

    private void AddCard(int index, int cardObjectIndex)
    {
    // TODO: change this to the deck location
        targetXLocation.Add(0f);
        targetYLocation.Add(0f);
        actionCardDisplays.Add(Instantiate(actionCardUIPrefab, this.transform));
        actionCards.Add(actionCardScriptableObjects[cardObjectIndex]);
        currentCardAmount++;
        UpdateDisplaySprite(index); 
        
        Debug.Log("An action card is instantiated");
    }

    private void Update()
    {
        CheckMousePosition();
        CalculateTargetLocation();
        UpdateCardPosition();
    }

    // Move the action card to the location they are supposed to be
    private void UpdateCardPosition()
    {
        for (int i = 0; i < actionCardDisplays.Count; i++)
        {
            Transform actionCardDisplayTransform = actionCardDisplays[i].transform;
            Vector3 target = new Vector3(
                targetXLocation[i],
                targetYLocation[i],
                actionCardDisplayTransform.localPosition.z);

            // If the card is returning to the original position
            if (actionCardDisplays[i].transform.localPosition.y > target.y)
            {
                actionCardDisplays[i].transform.localPosition = Vector3.Lerp(actionCardDisplays[i].transform.localPosition, target, cardMoveDownSpeed * Time.deltaTime);
            }
            // If the mouse is currently over the action card
            // Moving up to the desired position
            else
            {
                actionCardDisplays[i].transform.localPosition = Vector3.Lerp(actionCardDisplays[i].transform.localPosition, target, cardMoveUpSpeed * Time.deltaTime);
            }

        }
    }

    // Checks if the player clicks on the action cards
    // Works by the Unity Event System
    // Return the selected action card's index
    // Return -1 if none is selected
    public int CheckMousePosition()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition; 

        List<RaycastResult> results = new List<RaycastResult>();    
        raycaster.Raycast(pointerEventData, results);

        foreach(RaycastResult result in results)
        {
            // Check the index of the selected action card
            for (int i = 0; i < currentCardAmount; i++)
            {
                if (actionCardDisplays[i] == result.gameObject)
                {
                    if (debugging)
                    {
                        Debug.Log($"The index of the selected card is {i}");
                    }
                    SelectCard(i);
                    return i; 
                }
            }
        }
        SelectCard(-1); 
        return -1;
    }

    public void OnMouseHold()
    {
        int index = CheckMousePosition();
        if (index != -1)
        {
            draggedCardIndex = index; 
        }
    }

    public void OnMouseRelease()
    {
        draggedCardIndex = -1;
    }

    // Move up the selected card
    private void SelectCard(int index)
    {
        for (int i = 0; i < actionCardDisplays.Count; i++)
        {
            targetYLocation[i] = 0f; 
            if (i == index || i == draggedCardIndex)
            {
                targetYLocation[i] = selectYOffset;
            }
        }
    }

    // TODO: Change this to add a default card instead of showing one more existed card
    // Change current card amount by n
    //public void ChangeCurrentCardAmountBy(int n)
    //{
    //    currentCardAmount += n; 
    //    if (currentCardAmount <= 0 ||  currentCardAmount > maxCardLimit)
    //    {
    //        Debug.LogWarning("Card amount exceeds card amount limit"); 
    //    }
    //    currentCardAmount = Mathf.Clamp(currentCardAmount, 0, maxCardLimit);
    //    CalculateTargetLocation(); 
    //}

    // Calculates the x position the cards need to move to 
    // In order to maintain the gap while centering the group of cards
    private void CalculateTargetLocation()
    {
        if (currentCardAmount == 0)
            return; 

        //UpdateDisplayActivity(); 

        float totalDistance = (currentCardAmount - 1) * gapBetweenCards;
        targetXLocation[0] = -(totalDistance / 2f); 

        for (int i = 1; i < currentCardAmount; i++)
        {
            targetXLocation[i] = targetXLocation[i-1] + gapBetweenCards;    
        }

        //// New cards spawns in the location of the right most card
        //for (int i = currentCardAmount; i < currentCardAmount; i++)
        //{
        //    targetXLocation[i] = targetXLocation[currentCardAmount - 1]; 
        //}
    }

    //// Update all the current cards UI as active, vice versa
    //private void UpdateDisplayActivity()
    //{
    //    for (int i = 0; i < currentCardAmount; i++)
    //    {
    //        if (i >= currentCardAmount)
    //            actionCardDisplays[i].SetActive(false);
    //        else
    //            actionCardDisplays[i].SetActive(true);
    //    }
    //}
    private void UpdateDisplaySprite(int index)
    {
        actionCardDisplays[index].GetComponent<Image>().sprite = actionCards[index].sprite;
        
    }

    public int GetDraggedCardIndex()
    {
        return draggedCardIndex; 
    }
}
