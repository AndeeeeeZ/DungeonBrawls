using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private float gapBetweenCards, maxHandHorizontalLength, selectYOffset, waitTimeBetweenCards;

    [SerializeField]
    private Vector2Int deckLocation, discardPileLocation; 

    [HideInInspector]
    public int currentCardAmount;

    public int maxCardAmount, initialHandSize, cardMoveUpSpeed, cardMoveDownSpeed; 
    public static ActionCardController Instance { get; private set; }

    private PointerEventData pointerEventData;

    //private List<GameObject> actionCardDisplays;
    //private List<ActionCard> actionCards;
    //private List<float> targetXLocation; 
    //private List<float> targetYLocation;

    private List<ActionCardContainer> actionCardContainers;

    public ActionCardContainer actionCardContainerPrefab;
    public GameObject actionCardContainerParent, actionCardUIParent; 

    private int draggedCardIndex; 
   

    private void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //targetXLocation = new List<float>();
        //targetYLocation = new List<float>();
        //actionCardDisplays = new List<GameObject>(); 
        //actionCards = new List<ActionCard>();

        actionCardContainers = new List<ActionCardContainer>();

        currentCardAmount = 0; 
        draggedCardIndex = -1;

        StartCoroutine(InitializeActionCardList(initialHandSize)); 
        CalculateTargetLocation();
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
        for (int i = 0; i < actionCardContainers.Count; i++)
        {
            //Transform actionCardDisplayTransform = actionCardContainers[i].actionCardDisplay.transform;
            //Vector3 target = new Vector3(
            //    actionCardContainers[i].targetXLocation,
            //    actionCardContainers[i].targetYLocation,
            //    actionCardDisplayTransform.localPosition.z);

            //// If the card is returning to the original position
            //if (actionCardDisplays[i].transform.localPosition.y > target.y)
            //{
            //    actionCardDisplays[i].transform.localPosition = Vector3.Lerp(actionCardDisplays[i].transform.localPosition, target, cardMoveDownSpeed * Time.deltaTime);
            //}
            //// If the mouse is currently over the action card
            //// Moving up to the desired position
            //else
            //{
            //    actionCardDisplays[i].transform.localPosition = Vector3.Lerp(actionCardDisplays[i].transform.localPosition, target, cardMoveUpSpeed * Time.deltaTime);
            //}

            // CHECK: Does this really need to be called from this class? 
            actionCardContainers[i].UpdateCardDisplay();
        }
    }

    // Initialize with n action cards
    // Wait a certain amount of time between each card
    private IEnumerator InitializeActionCardList(int n)
    {
        for (int i = 0; i < n; i++)
        {
            AddCard(i, 0);
            yield return new WaitForSeconds(waitTimeBetweenCards); 
        }
    }

    // Add a card of the Scriptable Object of index cardObjectIndex at the end of the list
    public void AddCard(int cardObjectIndex)
    {
        AddCard(actionCardContainers.Count, cardObjectIndex);
    }

    // Add/Instantiate a card at index "index" of the Scriptable Object with the corresponding cardObjectIndex
    private void AddCard(int index, int cardObjectIndex)
    {
        if (currentCardAmount == maxCardAmount)
        {
            Debug.LogWarning("Current card amount reached the max hand size");
            return; 
        }

        //actionCardDisplays.Add(Instantiate(actionCardUIPrefab, this.transform));
        //actionCards.Add(actionCardScriptableObjects[cardObjectIndex]);

        actionCardContainers.Add(Instantiate(actionCardContainerPrefab, actionCardContainerParent.transform));
        actionCardContainers[actionCardContainers.Count - 1].ReceiveInfo(actionCardUIPrefab, actionCardUIParent, deckLocation.x, deckLocation.y, actionCardScriptableObjects[cardObjectIndex]); 
        currentCardAmount++;
        
        //// Spawns new card at deck location
        //actionCardDisplays[index].transform.localPosition = 
        //    new Vector3(deckLocation.x, deckLocation.y, 0); 

        UpdateDisplaySprite(index); 

        if (debugging)
            Debug.Log("An action card is instantiated");
    }

    // Remove the card that is currently being dragged
    public void RemoveSelectedCard()
    {
        if (draggedCardIndex != -1)
        {
            RemoveCard(draggedCardIndex);
        }
        else
        {
            Debug.LogWarning("No card is selected"); 
        }
    }

    // Remove the last card in the list
    public void RemoveLastCard()
    {
        RemoveCard(actionCardContainers.Count - 1);
    }

    // Remove card at index "index"
    private void RemoveCard(int index)
    {
        if (index >= actionCardContainers.Count)
        {
            Debug.LogWarning("Action card index out of bound");
            return; 
        }
        currentCardAmount--;
        actionCardContainers[index].DestorySelf();
        actionCardContainers.RemoveAt(index);
        if (debugging)
            Debug.Log($"Action card at index {index} is removed");
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
                if (actionCardContainers[i].actionCardDisplay == result.gameObject)
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
    // Move the selected card to the front, above all other cards
    private void SelectCard(int index)
    {
        for (int i = 0; i < actionCardContainers.Count; i++)
        {
            actionCardContainers[i].targetYLocation = 0f; 
            actionCardContainers[i].actionCardDisplay.transform.SetSiblingIndex(i); 
            if (i == index || i == draggedCardIndex)
            {
                actionCardContainers[i].targetYLocation = selectYOffset;
                // Move to the top of all card layers
                actionCardContainers[i].actionCardDisplay.transform.SetSiblingIndex(actionCardContainers.Count);
            }
        }
    }

    // Calculates the x position the cards need to move to 
    // In order to maintain the gap while centering the group of cards
    private void CalculateTargetLocation()
    {
        if (currentCardAmount == 0)
            return; 

        float totalDistance = (currentCardAmount - 1) * gapBetweenCards;
        float gap = gapBetweenCards; 

        // Checks if all the cards in hand is going to fit in the desired region
        if (totalDistance > maxHandHorizontalLength)
        {
            totalDistance = maxHandHorizontalLength;
            gap = maxHandHorizontalLength / (currentCardAmount - 1);     
        }

        actionCardContainers[0].targetXLocation = -(totalDistance / 2f); 

        for (int i = 1; i < currentCardAmount; i++)
        {
            actionCardContainers[i].targetXLocation = actionCardContainers[i-1].targetXLocation + gap;    
        }
    }
    private void UpdateDisplaySprite(int index)
    {
        actionCardContainers[index].UpdateSprite(); 
    }
    public int GetDraggedCardIndex()
    {
        return draggedCardIndex; 
    }
}
