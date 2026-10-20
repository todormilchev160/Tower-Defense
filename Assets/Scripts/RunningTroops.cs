using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunningTroops : MonoBehaviour
{
    [Header("Combat")]
    public float damage = 2f;
    public string enemyTag = "Enemy";

    [Header("Movement")]
    public Transform target;
    public float stoppingDistance = 0.5f;

    private NavMeshAgent navMeshAgent;

    // Tracks enemies already damaged by this soldier
    private HashSet<EnemyHealth> damagedEnemies = new HashSet<EnemyHealth>();

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (target != null)
        {
            navMeshAgent.stoppingDistance = stoppingDistance;
            navMeshAgent.SetDestination(target.position);
        }
    }

void Update()
{
    if (target == null || navMeshAgent == null)
        return;

    if (!navMeshAgent.isOnNavMesh)
        return;

    // Keep following the target
    navMeshAgent.SetDestination(target.position);

    // Check if the soldier has reached its destination
    if (!navMeshAgent.pathPending &&
        navMeshAgent.remainingDistance <= stoppingDistance &&
        (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude < 0.01f))
    {
        Destroy(gameObject);
    }
}

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(enemyTag))
            return;

        EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();

        if (enemyHealth == null || enemyHealth.IsDead())
            return;

        // Prevent damaging the same enemy multiple times
        if (damagedEnemies.Contains(enemyHealth))
            return;

        damagedEnemies.Add(enemyHealth);

        // Damage the enemy
        enemyHealth.TakeDamage(damage);
    }
}