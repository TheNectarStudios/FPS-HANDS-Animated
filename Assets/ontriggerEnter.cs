using UnityEngine;

public class EnableOnTrigger : MonoBehaviour
{
    public GameObject objectToEnable; // The GameObject to enable

    private void OnTriggerEnter(Collider other)  
    {
        // Check if the object that collided is the player
        if (other.CompareTag("Player"))
        {
            // Enable the object
            if (objectToEnable != null)
            {
                objectToEnable.SetActive(true);
            }
        }
    }
}
