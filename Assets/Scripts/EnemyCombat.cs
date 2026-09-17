using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float damage = 2f;
    [SerializeField] private float attackInterval = 1f;

    private NavMeshAgent agent;
    private EnemyWalk enemyWalk;
    private EnemyHealth enemyHealth;

    private Troops currentTroop;

    private bool engaged = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyWalk = GetComponent<EnemyWalk>();
        enemyHealth = GetComponent<EnemyHealth>();
    }

    public void Engage(Troops troop)
    {
        if (enemyHealth.IsDead())
            return;

        // Already fighting somebody
        if (engaged)
            return;

        currentTroop = troop;
        engaged = true;

        // Stop enemy movement
        agent.isStopped = true;
        agent.ResetPath();

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (engaged && currentTroop != null)
        {
            yield return new WaitForSeconds(attackInterval);

            if (currentTroop != null)
            {
                currentTroop.TakeDamage(damage);
            }
        }
    }

    public void TroopDied(Troops troop)
    {
        // Make sure this is the troop we're fighting
        if (troop != currentTroop)
            return;

        currentTroop = null;
        engaged = false;

        StopAllCoroutines();

        if (!enemyHealth.IsDead())
        {
            enemyWalk.ResumeMovement();
        }
    }

    public bool IsEngaged()
    {
        return engaged;
    }

    public bool IsDead()
    {
        return enemyHealth.IsDead();
    }
}