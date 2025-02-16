using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;  // Assign the zombie prefab in the Inspector
    public Transform player;         // Assign the player transform in the Inspector
    public float minDistance = 15f;  // Minimum spawn distance from player
    public float maxDistance = 30f;  // Maximum spawn distance from player
    public float despawnDistance = 100f; // Distance at which zombies get deleted
    public float spawnInterval = 5f; // Time between spawns

    private List<GameObject> activeZombies = new List<GameObject>(); // Track zombies

    private void Start()
    {
        InvokeRepeating(nameof(SpawnZombie), 2f, spawnInterval);
    }

    private void Update()
    {
        DespawnDistantZombies();
    }

    void SpawnZombie()
    {
        if (player == null || zombiePrefab == null) return;

        Vector3 spawnPosition;
        int attempts = 10; // Try multiple times to find a valid position

        for (int i = 0; i < attempts; i++)
        {
            spawnPosition = GetSpawnPosition();
            if (!IsInPlayerView(spawnPosition))
            {
                GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
                activeZombies.Add(zombie); // Add to list
                return;
            }
        }
    }

    Vector3 GetSpawnPosition()
    {
        float distance = Random.Range(minDistance, maxDistance);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * distance;
        return player.position + offset;
    }

    bool IsInPlayerView(Vector3 position)
    {
        Vector3 directionToSpawn = (position - player.position).normalized;
        float angleBetween = Vector3.Angle(player.forward, directionToSpawn);
        return angleBetween < 60f; // 60° field of view
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
