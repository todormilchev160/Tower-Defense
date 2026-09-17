using UnityEngine;
using UnityEngine.AI;

public class EnemyWalk : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private string destinationTag = "EnemyDestination";

    [Header("Random Path")]
    [Tooltip("How far ahead each random waypoint can be")]
    [SerializeField] private float waypointDistance = 6f;

    [Tooltip("How far left/right the waypoint can be")]
    [SerializeField] private float sidewaysRandomness = 4f;

    [Tooltip("How close the enemy must be to a waypoint")]
    [SerializeField] private float waypointReachDistance = 1f;

    [Tooltip("When this close to the final destination, go directly to it")]
    [SerializeField] private float directDestinationDistance = 5f;

    private NavMeshAgent agent;
    private Transform destination;

    private bool moving = true;
    private bool goingToFinalDestination = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject destinationObject =
            GameObject.FindGameObjectWithTag(destinationTag);

        if (destinationObject != null)
        {
            destination = destinationObject.transform;

            SetNextRandomWaypoint();
        }
    }

    void Update()
    {
        if (!moving || destination == null)
            return;

        // Don't do anything while NavMesh is calculating
        if (agent.pathPending)
            return;

        // Check if we've reached our current waypoint
        if (agent.remainingDistance <= waypointReachDistance)
        {
            if (!goingToFinalDestination)
            {
                SetNextRandomWaypoint();
            }
        }
    }

    void SetNextRandomWaypoint()
    {
        if (destination == null)
            return;

        float distanceToDestination = Vector3.Distance(
            transform.position,
            destination.position
        );

        // If we're close enough, just go directly to destination
        if (distanceToDestination <= directDestinationDistance)
        {
            goingToFinalDestination = true;
            agent.SetDestination(destination.position);
            return;
        }

        // Direction towards final destination
        Vector3 directionToDestination =
            (destination.position - transform.position).normalized;

        // Direction perpendicular to our movement
        Vector3 sideways = Vector3.Cross(
            Vector3.up,
            directionToDestination
        ).normalized;

        // Pick how far forward to go
        float forwardDistance = Random.Range(
            waypointDistance * 0.5f,
            waypointDistance
        );

        // Pick random left/right offset
        float sideOffset = Random.Range(
            -sidewaysRandomness,
            sidewaysRandomness
        );

        Vector3 desiredPoint =
            transform.position +
            directionToDestination * forwardDistance +
            sideways * sideOffset;

        // Find nearest valid position on NavMesh
        if (NavMesh.SamplePosition(
            desiredPoint,
            out NavMeshHit hit,
            waypointDistance,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // Couldn't find random point, go directly for now
            agent.SetDestination(destination.position);
        }
    }

    public void ResumeMovement()
    {
        if (destination == null)
            return;

        moving = true;
        agent.isStopped = false;

        // Continue with randomized movement
        SetNextRandomWaypoint();
    }

    public void StopMovement()
    {
        moving = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }
    }
}