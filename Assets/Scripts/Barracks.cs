using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Barracks : MonoBehaviour
{
    [Header("Troop")]
    [SerializeField] private GameObject troopPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;

    [Header("NavMesh")]
    [Tooltip("How far from this object Unity searches for a NavMesh")]
    [SerializeField] private float navMeshSearchDistance = 5f;

    // Shared by ALL TroopSpawners
    public static bool spawningUnlocked = false;

    // Coroutine belonging to this individual spawner
    private Coroutine spawnCoroutine;


    void Start()
    {
        // If spawning was already unlocked before
        // this spawner was created, start spawning.
        if (spawningUnlocked)
        {
            StartSpawning();
        }
    }


    // =====================================================
    // UNLOCK ALL SPAWNERS
    // =====================================================

    public static void UnlockSpawning()
    {
        spawningUnlocked = true;

        Barracks[] spawners =
            FindObjectsByType<Barracks>(
                FindObjectsSortMode.None
            );

        foreach (Barracks spawner in spawners)
        {
            spawner.StartSpawning();
        }

        Debug.Log("Troop spawning UNLOCKED");
    }


    // =====================================================
    // LOCK ALL SPAWNERS
    // =====================================================

    public static void LockSpawning()
    {
        spawningUnlocked = false;

        Barracks[] spawners =
            FindObjectsByType<Barracks>(
                FindObjectsSortMode.None
            );

        foreach (Barracks spawner in spawners)
        {
            spawner.StopSpawning();
        }

        Debug.Log("Troop spawning LOCKED");
    }


    // =====================================================
    // START THIS SPAWNER
    // =====================================================

    private void StartSpawning()
    {
        // Don't start if globally locked
        if (!spawningUnlocked)
            return;

        // Prevent multiple spawn coroutines
        if (spawnCoroutine != null)
            return;

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }


    // =====================================================
    // STOP THIS SPAWNER
    // =====================================================

    private void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);

            spawnCoroutine = null;
        }
    }


    // =====================================================
    // SPAWN LOOP
    // =====================================================

    IEnumerator SpawnRoutine()
    {
        while (spawningUnlocked)
        {
            yield return new WaitForSeconds(spawnInterval);

            // Check again because spawning could have
            // been locked while we were waiting.
            if (!spawningUnlocked)
                break;

            SpawnTroop();
        }

        spawnCoroutine = null;
    }


    // =====================================================
    // SPAWN TROOP
    // =====================================================

    void SpawnTroop()
    {
        if (troopPrefab == null)
        {
            Debug.LogWarning(
                "No troop prefab assigned to " +
                gameObject.name
            );

            return;
        }

        NavMeshHit hit;

        bool foundNavMesh = NavMesh.SamplePosition(
            transform.position,
            out hit,
            navMeshSearchDistance,
            NavMesh.AllAreas
        );

        if (foundNavMesh)
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
                "Could not find NavMesh near " +
                gameObject.name
            );
        }
    }


    // =====================================================
    // GAMEOBJECT ENABLE / DISABLE
    // =====================================================

    void OnEnable()
    {
        // If this object gets enabled after spawning
        // has already been unlocked, start it.
        if (spawningUnlocked)
        {
            StartSpawning();
        }
    }


    void OnDisable()
    {
        StopSpawning();
    }
}