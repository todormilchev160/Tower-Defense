using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour
{
    [SerializeField] private string destinationTag = "EnemyDestination";

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject destination = GameObject.FindGameObjectWithTag(destinationTag);

        if (destination != null)
        {
            agent.SetDestination(destination.transform.position);
        }
    }
}