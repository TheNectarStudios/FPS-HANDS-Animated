using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner2 : MonoBehaviour
{
    public GameObject zombiePrefab;  // Assign zombie prefab in Inspector
    public Transform player;         // Assign player Transform in Inspector
    public int maxZombies = 5;       // Maximum zombies to spawn at a time
    public float spawnRadius = 10f;  // Radius around spawner to spawn zombies
    public float spawnDistanceThreshold = 20f; // Player must be this far for zombies to spawn
    public float despawnDistance = 50f; // Despawn zombies if too far from spawner
    public float checkInterval = 5f;  // Time between spawn checks

    private List<GameObject> activeZombies = new List<GameObject>(); // Track spawned zombies

    private void Start()
    {
        InvokeRepeating(nameof(SpawnZombies), 2f, checkInterval);
    }

    private void Update()
    {
        DespawnDistantZombies();
    }

    void SpawnZombies()
    {
        if (player == null || zombiePrefab == null) return;

        float playerDistance = Vector3.Distance(transform.position, player.position);

        // Only spawn if the player is far enough away
        if (playerDistance < spawnDistanceThreshold) return;

        int zombiesToSpawn = maxZombies - activeZombies.Count;
        if (zombiesToSpawn <= 0) return;

        for (int i = 0; i < zombiesToSpawn; i++)
        {
            Vector3 spawnPosition;
            int attempts = 10; // Try multiple times to find a valid position

            for (int j = 0; j < attempts; j++)
            {
                spawnPosition = GetSpawnPosition();
                if (!IsInPlayerView(spawnPosition))
                {
                    GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
                    activeZombies.Add(zombie);
                    break;
                }
            }
        }
    }

    Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        float distance = Random.Range(3f, spawnRadius); // Random spawn within radius

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        return transform.position + offset;
    }

    bool IsInPlayerView(Vector3 position)
    {
        Vector3 directionToSpawn = (position - player.position).normalized;
        float angleBetween = Vector3.Angle(player.forward, directionToSpawn);
        return angleBetween < 60f; // Avoid spawning in a 60° field of view
    }

    void DespawnDistantZombies()
    {
        for (int i = activeZombies.Count - 1; i >= 0; i--)
        {
            if (activeZombies[i] == null)
            {
                activeZombies.RemoveAt(i);
                continue;
            }

            float distance = Vector3.Distance(transform.position, activeZombies[i].transform.position);
            if (distance > despawnDistance)
            {
                Destroy(activeZombies[i]);
                activeZombies.RemoveAt(i);
            }
        }
    }
}
