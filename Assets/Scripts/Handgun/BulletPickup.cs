using UnityEngine;

public class BulletPickup : MonoBehaviour
{
    public int bulletAmount = 20; // Amount of bullets per pickup
    public AudioClip pickupSound; // Assign in Inspector
    public float pickupRange = 3f; // Maximum distance to pick up bullets

    private Transform player;
    private bool isPickedUp = false; // Prevents multiple pickups

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
            PickupBullets();
        }
    }

    private void PickupBullets()
    {
        isPickedUp = true; // Prevents multiple pickups

        BulletManager bulletManager = FindObjectOfType<BulletManager>(); // Find the bullet manager
        if (bulletManager != null)
        {
            bulletManager.totalAmmo += bulletAmount; // Add bullets
            bulletManager.UpdateBulletUI(); // Update UI
        }

        // Play pickup sound properly
        if (pickupSound != null)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = pickupSound;
            audioSource.Play();
            Destroy(gameObject, pickupSound.length); // Destroy after sound finishes
        }
        else
        {
            Destroy(gameObject); // Destroy immediately if no sound
        }
    }
}
