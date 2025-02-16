using UnityEngine;
using EvolveGames;
public class MedikitPickup : MonoBehaviour
{
    public int medikitAmount = 1; // Number of medikits per pickup
    public AudioClip pickupSound; // Assign in Inspector
    public float pickupRange = 3f; // Max distance to pick up

    private Transform player;
    private bool isPickedUp = false;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null || isPickedUp) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= pickupRange && Input.GetKeyDown(KeyCode.E))
        {
            PickupMedikit();
        }
    }

    private void PickupMedikit()
    {
        isPickedUp = true;

        ItemChange itemChange = FindObjectOfType<ItemChange>(); // Find ItemChange script
        if (itemChange != null)
        {
            itemChange.IncreaseMedikitCount(medikitAmount);
        }

        // Play pickup sound before destroying
        if (pickupSound != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = pickupSound;
            audioSource.Play();
            Destroy(gameObject, pickupSound.length);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
