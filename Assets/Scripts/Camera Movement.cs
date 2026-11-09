using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float cameraOffsetX = 2f; // Horizontal offset from the player
    [SerializeField] private float cameraOffsetY = 1f; // Vertical offset from the player

    private Transform playerTransform;
    private Vector2 offset;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        offset = new Vector2(cameraOffsetX, cameraOffsetY);
    }

    private void LateUpdate()
    {
        if (playerTransform != null)
        {
            Vector3 offsetPosition = new Vector3(playerTransform.position.x + offset.x, playerTransform.position.y + offset.y, transform.position.z);
            transform.position = offsetPosition;
        }
    }
}
