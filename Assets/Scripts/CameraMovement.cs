using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    [SerializeField]
    private float cameraSpeed; 

    private void Update()
    {
        // Move camera to player position
        Vector3 target = new Vector3(player.position.x, player.position.y, transform.position.z); 
        transform.position = Vector3.Lerp(transform.position, target, cameraSpeed * Time.deltaTime);
    }
}
