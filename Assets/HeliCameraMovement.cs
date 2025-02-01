using UnityEngine;

public class HeliCameraMovement : MonoBehaviour
{
    public float maxRotationAngle = 30f; // Maximum angle in each direction
    public float rotationSpeed = 2f; // Speed of rotation

    private Vector3 initialForward;
    private float yaw = 0f; // Horizontal rotation
    private float pitch = 0f; // Vertical rotation

    void Start()
    {
        // Store the initial forward direction
        initialForward = transform.forward;
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        // Adjust yaw and pitch based on input
        yaw += mouseX;
        pitch -= mouseY;

        // Clamp yaw and pitch to the defined range
        yaw = Mathf.Clamp(yaw, -maxRotationAngle, maxRotationAngle);
        pitch = Mathf.Clamp(pitch, -maxRotationAngle, maxRotationAngle);

        // Apply rotation based on clamped values
        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.LookRotation(targetRotation * initialForward);
    }
}
