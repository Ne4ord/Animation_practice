using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Distance")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    [Header("Rotation")]
    public float mouseSensitivity = 3f;
    public float minVerticalAngle = -5f;
    public float maxVerticalAngle = 60f;

    [Header("Smooth")]
    public float smoothSpeed = 10f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }

    void LateUpdate() 
    {
        if (player == null)
            return;

        
        if (Time.timeScale == 0f)
            return;

        
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, minVerticalAngle, maxVerticalAngle);

        
        distance -= Input.GetAxis("Mouse ScrollWheel") * 3f;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

            
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 targetPosition = player.position + rotation * new Vector3(0, 0, -distance);

        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }
}
