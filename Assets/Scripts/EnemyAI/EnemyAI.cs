using UnityEngine;
using UnityEngine.AI;
using System.Collections ; 
public class ZombieController : MonoBehaviour
{
    private Animator animator;
    private Transform player;
    private NavMeshAgent agent;

    [Header("Movement Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;

    [Header("Audio Settings")]
    public AudioSource continuousRoar;
    public AudioSource hyperRoar;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    private Vector3 randomPatrolPoint;
    private float patrolTimer;

    private bool isPatrolling = true;
    private bool isChasing = false;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Initialize health and patrolling
        currentHealth = maxHealth;
        SelectRandomPatrolPoint();
        agent.speed = patrolSpeed;
    }

    void Update()
    {
        if (isDead)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartAttacking();
        }
        else if (distanceToPlayer <= detectionRange || isChasing) // Keep chasing if already chasing
        {
            StartChasing();
        }
        else
        {
            StartPatrolling();
        }

        UpdateAnimatorParameters();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartChasing(); // Force zombie to chase the player when hit
        }
    }
    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        continuousRoar.Stop();
        hyperRoar.Stop();

        agent.isStopped = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Disable the Animator after death animation finishes
        StartCoroutine(DisableAnimatorAfterDeath());
    }

    private IEnumerator DisableAnimatorAfterDeath()
    {
        yield return new WaitForSeconds(3f); // Adjust to match animation length
        animator.enabled = false; // Stops looping issues
    }

    private void StartPatrolling()
    {
        if (!continuousRoar.isPlaying)
        {
            hyperRoar.Stop();
            continuousRoar.Play();
        }

        isPatrolling = true;
        isChasing = false;
        isAttacking = false;

        animator.ResetTrigger("attack");
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        Patrol();
    }

    private void StartChasing()
    {
        if (!hyperRoar.isPlaying)
        {
            continuousRoar.Stop();
            hyperRoar.Play();
        }

        isPatrolling = false;
        isChasing = true;
        isAttacking = false;

        animator.ResetTrigger("attack");
        agent.isStopped = false;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    private void StartAttacking()
    {
        if (!hyperRoar.isPlaying)
        {
            continuousRoar.Stop();
            hyperRoar.Play();
        }

        isPatrolling = false;
        isChasing = false;
        isAttacking = true;

        agent.isStopped = true;
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        animator.SetTrigger("attack");
    }

    private void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (agent.remainingDistance < 0.5f || patrolTimer >= patrolWaitTime)
        {
            SelectRandomPatrolPoint();
            patrolTimer = 0f;
        }
    }

    private void SelectRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        randomDirection.y = transform.position.y;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            randomPatrolPoint = hit.position;
            agent.SetDestination(randomPatrolPoint);
        }
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("patrolling", isPatrolling);
        animator.SetBool("chasing", isChasing);
        animator.SetBool("attacking", isAttacking);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(20);
            Debug.Log("Zombie hit by bullet!");
            Debug.Log("Current Health: " + currentHealth);

            Destroy(collision.gameObject);
        }
    }
}
