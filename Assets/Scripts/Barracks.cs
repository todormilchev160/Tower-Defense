using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Barracks : MonoBehaviour
{
    [Header("Troop")]
    [SerializeField] private GameObject troopPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 5f;

    [SerializeField] private int maxTroops = 5;

    [Header("NavMesh")]
    [SerializeField] private float navMeshSearchDistance = 5f;

    [Header("Rally Point")]
    [Tooltip("Maximum distance from this Barracks where a rally point can be placed")]
    [SerializeField] private float rallyRange = 15f;

    private int currentTroops = 0;

    private Vector3 rallyPosition;
    private bool hasRallyPoint = false;

    private Coroutine spawnCoroutine;


    void Start()
    {
        StartSpawning();
    }


    // =====================================================
    // SPAWNING
    // =====================================================

    private void StartSpawning()
    {
        if (spawnCoroutine != null)
            return;

        spawnCoroutine =
            StartCoroutine(SpawnRoutine());
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
            yield return new WaitForSeconds(
                spawnInterval
            );

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


        bool foundNavMesh =
            NavMesh.SamplePosition(
                transform.position,
                out hit,
                navMeshSearchDistance,
                NavMesh.AllAreas
            );


        if (!foundNavMesh)
            return;


        GameObject spawnedTroop =
            Instantiate(
                troopPrefab,
                hit.position,
                transform.rotation
            );


        Troops troopScript =
            spawnedTroop.GetComponent<Troops>();


        if (troopScript != null)
        {
            troopScript.SetBarracks(this);
        }


        currentTroops++;
    }


    // =====================================================
    // TROOP DIED
    // =====================================================

    public void TroopDied()
    {
        currentTroops--;

        if (currentTroops < 0)
            currentTroops = 0;
    }


    public int GetCurrentTroops()
    {
        return currentTroops;
    }


    // =====================================================
    // RALLY POINT
    // =====================================================

    public void SetRallyPoint(Vector3 position)
    {
        rallyPosition = position;

        hasRallyPoint = true;


        Debug.Log(
            name +
            " rally point = " +
            rallyPosition
        );
    }


    public Vector3 GetRallyPosition()
    {
        return rallyPosition;
    }


    public bool HasRallyPoint()
    {
        return hasRallyPoint;
    }


    public float GetRallyRange()
    {
        return rallyRange;
    }


    // =====================================================
    // DEBUG RANGE
    // =====================================================

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            rallyRange
        );
    }


    // =====================================================
    // ENABLE / DISABLE
    // =====================================================

    void OnEnable()
    {
        StartSpawning();
    }


    void OnDisable()
    {
        StopSpawning();
    }
}