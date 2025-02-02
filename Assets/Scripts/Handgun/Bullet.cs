using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 2f; // Time before the bullet is destroyed automatically
    public GameObject hitEffect; // Assign a particle effect prefab in the Inspector

    void Start()
    {
        Destroy(gameObject, lifeTime); // Destroy bullet after 'lifeTime' seconds
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has the "Zombie" tag
        if (collision.gameObject.CompareTag("Zombie"))
        {
            if (hitEffect != null)
            {
                // Spawn the particle effect at the point of impact
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                Destroy(effect, 0.5f); // Destroy the particle effect after 0.5 sec
            }

            Destroy(gameObject); // Destroy the bullet on impact
        }
    }
}
