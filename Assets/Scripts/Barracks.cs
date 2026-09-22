using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Barracks : MonoBehaviour
{
    [Header("Troop")]
    [SerializeField] private GameObject troopPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;

    [Tooltip("Maximum number of troops that can exist at once across ALL barracks")]
    [SerializeField] private int maxTroops = 5;

    [Header("NavMesh")]
    [Tooltip("How far from this object Unity searches for a NavMesh")]
    [SerializeField] private float navMeshSearchDistance = 5f;

    // Shared by ALL Barracks
    public static bool spawningUnlocked = false;

    // Number of troops currently alive
    private static int currentTroops = 0;

    // Coroutine belonging to this individual Barracks
    private Coroutine spawnCoroutine;


    void Start()
    {
        if (spawningUnlocked)
        {
            StartSpawning();
        }
    }


    // =====================================================
    // UNLOCK ALL BARRACKS
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
    // LOCK ALL BARRACKS
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
    // START THIS BARRACKS
    // =====================================================

    private void StartSpawning()
    {
        if (!spawningUnlocked)
            return;

        // Prevent multiple spawn coroutines
        if (spawnCoroutine != null)
            return;

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }


    // =====================================================
    // STOP THIS BARRACKS
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

            if (!spawningUnlocked)
                break;

            // Only spawn if we haven't reached the cap
            if (currentTroops < maxTroops)
            {
                SpawnTroop();
            }
        }

        spawnCoroutine = null;
    }


    // =====================================================
    // SPAWN TROOP
    // =====================================================

    void SpawnTroop()
    {
        if (troopPrefab == null)
            return;

        // Safety check
        if (currentTroops >= maxTroops)
            return;

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

            currentTroops++;
        }
    }


  

    public static void TroopDied()
    {
        currentTroops--;

        if (currentTroops < 0)
            currentTroops = 0;

    }
    
    public static int GetCurrentTroops()
    {
        return currentTroops;
    }


    
    void OnEnable()
    {
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