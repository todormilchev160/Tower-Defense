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
        // IMPORTANT:
        // Do absolutely nothing while combat has stopped us.
        if (!moving)
            return;

        if (destination == null)
            return;

        if (agent == null || !agent.isOnNavMesh)
            return;

        if (agent.pathPending)
            return;

        // If we're going directly to the final destination,
        // don't create any more random waypoints.
        if (goingToFinalDestination)
            return;

        if (!agent.hasPath)
            return;

        if (agent.remainingDistance <= waypointReachDistance)
        {
            SetNextRandomWaypoint();
        }
    }

    void SetNextRandomWaypoint()
    {
        if (!moving)
            return;

        if (destination == null)
            return;

        if (agent == null || !agent.isOnNavMesh)
            return;

        float distanceToDestination = Vector3.Distance(
            transform.position,
            destination.position
        );

        // Close enough -> go directly to final destination
        if (distanceToDestination <= directDestinationDistance)
        {
            goingToFinalDestination = true;

            agent.isStopped = false;
            agent.SetDestination(destination.position);

            return;
        }

        Vector3 directionToDestination =
            destination.position - transform.position;

        // Keep direction horizontal
        directionToDestination.y = 0f;
        directionToDestination.Normalize();

        Vector3 sideways = Vector3.Cross(
            Vector3.up,
            directionToDestination
        ).normalized;

        float forwardDistance = Random.Range(
            waypointDistance * 0.5f,
            waypointDistance
        );

        float sideOffset = Random.Range(
            -sidewaysRandomness,
            sidewaysRandomness
        );

        Vector3 desiredPoint =
            transform.position +
            directionToDestination * forwardDistance +
            sideways * sideOffset;

        if (NavMesh.SamplePosition(
            desiredPoint,
            out NavMeshHit hit,
            waypointDistance,
            NavMesh.AllAreas))
        {
            agent.isStopped = false;
            agent.SetDestination(hit.position);
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(destination.position);
        }
    }

    // Called when combat starts
    public void StopMovement()
    {
        moving = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }

    // Called when enemy wins combat
    public void ResumeMovement()
    {
        if (destination == null)
            return;

        moving = true;
        goingToFinalDestination = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;

            SetNextRandomWaypoint();
        }
    }
}