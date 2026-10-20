using UnityEngine;
using UnityEngine.AI;

public class SpawnElites : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject elitePrefab;
    public int spawnAmount = 3;
    public float spawnSpacing = 1.5f;

    [Header("NavMesh")]
    public float navMeshSearchDistance = 5f;

    public void SpawnTroops()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            // Create an offset for each troop
            Vector3 offset = new Vector3(
                (i - (spawnAmount - 1) / 2f) * spawnSpacing,
                0f,
                0f
            );

            Vector3 desiredPosition = transform.position + offset;

            // Find nearest valid NavMesh position
            if (NavMesh.SamplePosition(
                desiredPosition,
                out NavMeshHit hit,
                navMeshSearchDistance,
                NavMesh.AllAreas))
            {
                Instantiate(
                    elitePrefab,
                    hit.position,
                    transform.rotation
                );
            }
            else
            {
                Debug.LogWarning("No NavMesh found near spawn position!");
            }
        }
    }
}
