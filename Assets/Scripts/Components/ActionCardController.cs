using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic; 

public class ActionCardController : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private GraphicRaycaster raycaster;

    [SerializeField]
    private GameObject[] actionCards;

    public int maxCardLimit, currentCardAmount;

    private PointerEventData pointerEventData;

    private void Start()
    {
        maxCardLimit = actionCards.Length;
    }

    public void CheckMouseClick()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition; 

        List<RaycastResult> results = new List<RaycastResult>();    
        raycaster.Raycast(pointerEventData, results);

        foreach(RaycastResult result in results)
        {
            Debug.Log("Mouse clicked on something"); 
        }
    }
}
