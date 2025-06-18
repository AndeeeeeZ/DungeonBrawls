using System.Runtime.CompilerServices;
using UnityEngine;

public class ArrowIndictor : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3 startPosition; 

    private bool rendering; 
    private void Start()
    {
        rendering = false; 
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (rendering) 
        { 
            RenderLine();
        }
    }

    // Renders line
    public void RenderLine()
    {
        lineRenderer.positionCount = 2; 
        Vector3[] points = new Vector3[2];
        points[0] = startPosition;
        points[1] = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Sets z position to 0 so the line is properly rendered in game view
        points[0].z = 0f; 
        points[1].z = 0f;

        lineRenderer.SetPositions(points); 
    }

    // Begins rendering and sets start position
    // Only renders if currently dragging a card
    public void BeginRenderLine()
    {
        if (InputSystem.Instance.IsDraggingACard())
        {
            rendering = true; 
            startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
    }

    // Ends rendering
    public void EndRenderLine()
    {
        rendering = false; 
        lineRenderer.positionCount = 0; 
    }
}
