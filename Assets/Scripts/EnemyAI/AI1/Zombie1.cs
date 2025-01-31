using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    public float detectRange = 15f;
    public float attackRange = 2f;
    public int health = 100;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;
    private bool isDead = false;
    private bool hasScreamed = false;
    private int currentRunIndex = 1; // Store the current run animation index

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            Attack();
        }
        else if (distance <= detectRange)
        {
            if (!hasScreamed)
            {
                Scream();
            }
            else
            {
                ChasePlayer();
            }
        }
        else
        {
            StopChasing();
        }
    }

    void Scream()
    {
        hasScreamed = true;
        animator.SetBool("Scream", true);

        // Wait for scream animation to finish before running
        Invoke(nameof(StartRunning), 2f); // Adjust delay based on scream animation duration
    }

    void StartRunning()
    {
        animator.SetBool("Scream", false); // Disable scream
        ChasePlayer();
    }

    void ChasePlayer()
    {
        ResetRunAnimations();
        currentRunIndex = Random.Range(1, 7);  // Randomly pick run1 to run6
        animator.SetBool("run" + currentRunIndex, true);
        agent.SetDestination(player.position);
    }

    void StopChasing()
    {
        ResetRunAnimations();
        agent.ResetPath();
    }

    void Attack()
    {
        StopChasing();
        animator.SetBool("Attack", true);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        ResetRunAnimations();
        int randomDeath = Random.Range(1, 4); // Randomly pick death1 to death3
        animator.SetBool("death" + randomDeath, true);
        agent.isStopped = true;
        Destroy(gameObject, 5f); // Destroy zombie after 5 seconds
    }

    void ResetRunAnimations()
    {
        for (int i = 1; i <= 6; i++) animator.SetBool("run" + i, false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(20);
            Destroy(other.gameObject);  // Destroy bullet on impact
        }
    }
}
