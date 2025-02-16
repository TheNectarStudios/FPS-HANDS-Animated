using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f; // Time before the bullet is destroyed automatically
    public GameObject hitEffect; // Assign a particle effect prefab in the Inspector
    public int bulletDamage = 25; // Damage dealt by the bullet

    void Start()
    {
        Destroy(gameObject, lifeTime); // Destroy bullet after 'lifeTime' seconds
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object is a Zombie
        if (collision.gameObject.CompareTag("Zombie"))
        {
            // Try to get the ZombieAI script
            ZombieAI zombie = collision.gameObject.GetComponent<ZombieAI>();
            if (zombie != null)
            {
                zombie.TakeDamage(bulletDamage); // Reduce health
            }

            // Spawn the hit effect (if assigned)
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, 0.5f); // Destroy effect after 0.5 sec
            }

            Destroy(gameObject); // Destroy bullet on impact
        }
    }
}
