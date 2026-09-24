using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Barracks : MonoBehaviour
{
    [Header("Troop")]
    [SerializeField] private GameObject troopPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;

    [Tooltip("Maximum troops THIS barracks can have alive")]
    [SerializeField] private int maxTroops = 5;

    [Header("NavMesh")]
    [SerializeField] private float navMeshSearchDistance = 5f;

    // NOT STATIC anymore
    // Each Barracks has its own count
    private int currentTroops = 0;

    private Coroutine spawnCoroutine;


    void Start()
    {
        StartSpawning();
    }


    private void StartSpawning()
    {
        if (spawnCoroutine != null)
            return;

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }


    private void StopSpawning()
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
            yield return new WaitForSeconds(spawnInterval);

            if (currentTroops < maxTroops)
            {
                SpawnTroop();
            }
        }
    }


    void SpawnTroop()
    {
        if (troopPrefab == null)
            return;

        if (currentTroops >= maxTroops)
            return;

        NavMeshHit hit;

        bool foundNavMesh = NavMesh.SamplePosition(
            transform.position,
            out hit,
            navMeshSearchDistance,
            NavMesh.AllAreas
        );

        if (!foundNavMesh)
            return;

        GameObject spawnedTroop = Instantiate(
            troopPrefab,
            hit.position,
            transform.rotation
        );

        // Get the Troops script
        Troops troopScript = spawnedTroop.GetComponent<Troops>();

        if (troopScript != null)
        {
            // Tell the troop which Barracks created it
            troopScript.SetBarracks(this);
        }

        currentTroops++;

        Debug.Log(
            name + " troops: " +
            currentTroops + "/" + maxTroops
        );
    }


    // Called ONLY by troops belonging to this Barracks
    public void TroopDied()
    {
        currentTroops--;

        if (currentTroops < 0)
            currentTroops = 0;

        Debug.Log(
            name + " troops: " +
            currentTroops + "/" + maxTroops
        );
    }


    public int GetCurrentTroops()
    {
        return currentTroops;
    }


    void OnEnable()
    {
        StartSpawning();
    }


    void OnDisable()
    {
        StopSpawning();
    }
}