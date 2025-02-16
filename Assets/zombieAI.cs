using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    private Transform player; // Player Transform (found by tag)
    private NavMeshAgent agent;

    [Header("Zombie Settings")]
    public float chaseRange = 15f;  // Start chasing when within this distance
    public float attackRange = 2f;  // Attack when close
    public float attackCooldown = 1.5f;  // Time between attacks
    public int attackDamage = 20;  // Damage per attack

    private float lastAttackTime = 0;
    private bool isDead = false;
    private bool isAttacking = false;

    [Header("Zombie Health")]
    public int health = 100;

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Find the player automatically by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure your player has the tag 'Player'.");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("Attack", false); // Stop attacking if far
        }
    }

    void ChasePlayer()
    {
        if (agent.enabled)
        {
            agent.SetDestination(player.position);
            animator.SetBool("isWalking", true);
            animator.SetBool("Attack", false); // Stop attack when moving
        }
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            isAttacking = true;
            animator.SetBool("Attack", true);
            
            // Deal damage if player has a health script
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        Debug.Log("Zombie took damage! Current health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        agent.enabled = false; // Disable movement
        Destroy(gameObject, 5f); // Destroy after 5 sec
    }

    // Handle damage from bullets
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Bullet")
        {
            TakeDamage(25); // Reduce health by 25 per bullet hit
            Destroy(other.gameObject); // Destroy the bullet on impact
        }
    }
}
