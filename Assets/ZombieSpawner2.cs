using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class ZombieSpawner2 : MonoBehaviour
{
    public GameObject zombiePrefab;  // Assign the zombie prefab in the Inspector
    public Transform player;         // Assign the player transform in the Inspector
    public float spawnRadius = 10f;  // Radius around this object to spawn zombies
    public float activationDistance = 20f; // Distance at which player activates spawner
    public float despawnDistance = 100f; // Distance at which zombies get deleted
    public float spawnInterval = 5f; // Time between spawns
    public float navMeshCheckRadius = 2f; // Radius for NavMesh check

    private List<GameObject> activeZombies = new List<GameObject>(); // Track zombies
    private bool isActive = false;

    private void Start()
    {
        InvokeRepeating(nameof(CheckPlayerDistance), 1f, 1f); // Check player distance periodically
        InvokeRepeating(nameof(SpawnZombie), 2f, spawnInterval);
    }

    private void Update()
    {
        DespawnDistantZombies();
    }

    void CheckPlayerDistance()
    {
        if (player == null) return;
        isActive = Vector3.Distance(player.position, transform.position) <= activationDistance;
    }

    void SpawnZombie()
    {
        if (!isActive || zombiePrefab == null) return;

        Vector3 spawnPosition;
        int attempts = 10; // Try multiple times to find a valid position

        for (int i = 0; i < attempts; i++)
        {
            spawnPosition = GetSpawnPosition();
            if (IsOnNavMesh(spawnPosition))
            {
                GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
                activeZombies.Add(zombie); // Add to list
                return;
            }
        }
    }

    Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
        return transform.position + offset;
    }

    bool IsOnNavMesh(Vector3 position)
    {
        NavMeshHit hit;
        return NavMesh.SamplePosition(position, out hit, navMeshCheckRadius, NavMesh.AllAreas);
    }

    void DespawnDistantZombies()
    {
        for (int i = activeZombies.Count - 1; i >= 0; i--)
        {
            if (activeZombies[i] == null)
            {
                activeZombies.RemoveAt(i); // Remove destroyed zombies
                continue;
            }

            float distance = Vector3.Distance(player.position, activeZombies[i].transform.position);
            if (distance > despawnDistance)
            {
                Destroy(activeZombies[i]);
                activeZombies.RemoveAt(i);
            }
        }
    }
}
