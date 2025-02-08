using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public Transform player; // Assign player transform here
    public float height = 20f; // Adjust the height of the camera
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        // Follow player position but maintain height
        Vector3 newPosition = player.position + Vector3.up * height;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * smoothSpeed);

        // Keep looking downward
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
