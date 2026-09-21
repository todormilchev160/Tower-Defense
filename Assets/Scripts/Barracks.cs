using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Barracks : MonoBehaviour
{
    [Header("Troop")]
    [SerializeField] private GameObject troopPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Seconds between each troop spawn")]
    [SerializeField] private float spawnInterval = 5f;

    [Header("NavMesh")]
    [Tooltip("How far from this object to search for the NavMesh")]
    [SerializeField] private float navMeshSearchDistance = 5f;

    private Coroutine spawnCoroutine;

    void OnEnable()
    {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnTroop();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnTroop()
    {
        if (troopPrefab == null)
        {
            Debug.LogWarning("No troop prefab assigned!");
            return;
        }

        NavMeshHit hit;

        // Find the closest NavMesh point
        if (NavMesh.SamplePosition(
            transform.position,
            out hit,
            navMeshSearchDistance,
            NavMesh.AllAreas))
        {
            Instantiate(
                troopPrefab,
                hit.position,
                transform.rotation
            );
        }
        else
        {
            Debug.LogWarning(
                "Could not find NavMesh near " + gameObject.name
            );
        }
    }
}