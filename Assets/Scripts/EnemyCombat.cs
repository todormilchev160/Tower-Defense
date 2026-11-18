
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Troop Combat")]
    [SerializeField] private float damage = 2f;
    [SerializeField] private float attackInterval = 1f;

    [Header("Barricade Combat")]
    [SerializeField] private float barricadeDetectionRange = 5f;
    [SerializeField] private float barricadeAttackDistance = 2f;
    [SerializeField] private float barricadeDamage = 5f;
    [SerializeField] private float barricadeAttackInterval = 1f;

    public Animator animator;

    private NavMeshAgent agent;
    private EnemyWalk enemyWalk;
    private EnemyHealth enemyHealth;

    private Troops currentTroop;
    private bool engaged = false;

    private Barricade currentBarricade;
    private Coroutine barricadeAttackCoroutine;
    private bool attackingBarricade = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyWalk = GetComponent<EnemyWalk>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (agent != null)
        {
            agent.avoidancePriority = 50;
            agent.obstacleAvoidanceType =
                ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
    }

    void Update()
    {
        if (IsDead())
            return;

        if (attackingBarricade && currentBarricade == null)
        {
            FinishBarricadeAttack();
            return;
        }

        if (engaged)
            return;

        FindNearestBarricade();

        if (currentBarricade != null)
        {
            HandleBarricade();
        }
    }

    // ==========================================
    // NAVMESH COMBAT CONTROL
    // ==========================================

    void StopForCombat()
    {
        if (agent == null || !agent.enabled)
            return;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        // Other agents no longer avoid this enemy
        agent.enabled = false;
    }

    void ResumeAfterCombat()
    {
        if (agent == null)
            return;

        if (!agent.enabled)
            agent.enabled = true;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(
                "Enemy could not resume: not on NavMesh!"
            );
            return;
        }

        agent.isStopped = false;

        if (enemyWalk != null)
        {
            enemyWalk.ResumeMovement();
        }
    }

    // ==========================================
    // ENGAGE TROOP
    // ==========================================

    public void Engage(Troops troop)
    {
        if (IsDead())
            return;

        if (engaged || attackingBarricade)
            return;

        if (troop == null)
            return;

        currentTroop = troop;
        engaged = true;

        StopForCombat();

        if (animator != null)
        {
            animator.SetBool("Attack", true);
        }

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (engaged)
        {
            yield return new WaitForSeconds(
                attackInterval
            );

            if (currentTroop == null)
                break;

            currentTroop.TakeDamage(damage);
        }
    }

    // ==========================================
    // TROOP DIED
    // ==========================================

    public void TroopDied(Troops troop)
    {
        if (troop != currentTroop)
            return;

        currentTroop = null;
        engaged = false;

        if (animator != null)
        {
            animator.SetBool("Attack", false);
        }

        StopAllCoroutines();

        if (!IsDead())
        {
            ResumeAfterCombat();
        }
    }

    // ==========================================
    // FIND BARRICADE
    // ==========================================

    void FindNearestBarricade()
    {
        if (currentBarricade != null)
            return;

        Barricade[] barricades =
            FindObjectsByType<Barricade>(
                FindObjectsSortMode.None
            );

        float closestDistance = Mathf.Infinity;
        Barricade closestBarricade = null;

        foreach (Barricade barricade in barricades)
        {
            if (barricade == null)
                continue;

            if (barricade.IsDead())
                continue;

            float distance = Vector3.Distance(
                transform.position,
                barricade.transform.position
            );

            if (distance > barricadeDetectionRange)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBarricade = barricade;
            }
        }

        currentBarricade = closestBarricade;
    }

    // ==========================================
    // HANDLE BARRICADE
    // ==========================================

    void HandleBarricade()
    {
        if (currentBarricade == null)
        {
            if (attackingBarricade)
                FinishBarricadeAttack();

            return;
        }

        if (currentBarricade.IsDead())
        {
            FinishBarricadeAttack();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            currentBarricade.transform.position
        );

        // Move toward barricade
        if (distance > barricadeAttackDistance)
        {
            if (!agent.enabled)
                agent.enabled = true;

            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;

                agent.SetDestination(
                    currentBarricade.transform.position
                );
            }

            return;
        }

        // Stop when within attack range
        if (!attackingBarricade)
        {
            attackingBarricade = true;

            StopForCombat();

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }

            barricadeAttackCoroutine =
                StartCoroutine(AttackBarricadeRoutine());
        }
    }

    // ==========================================
    // ATTACK BARRICADE
    // ==========================================

    IEnumerator AttackBarricadeRoutine()
    {
        while (attackingBarricade)
        {
            yield return new WaitForSeconds(
                barricadeAttackInterval
            );

            if (currentBarricade == null ||
                currentBarricade.IsDead())
            {
                FinishBarricadeAttack();
                yield break;
            }

            currentBarricade.TakeDamage(
                barricadeDamage
            );

            if (currentBarricade == null ||
                currentBarricade.IsDead())
            {
                FinishBarricadeAttack();
                yield break;
            }
        }
    }

    // ==========================================
    // FINISH BARRICADE ATTACK
    // ==========================================

    void FinishBarricadeAttack()
    {
        currentBarricade = null;
        barricadeAttackCoroutine = null;
        attackingBarricade = false;

        if (!IsDead())
        {
            ResumeAfterCombat();
        }
    }

    // ==========================================
    // DEATH
    // ==========================================

    public void Die()
    {
        attackingBarricade = false;
        currentBarricade = null;
        barricadeAttackCoroutine = null;

        engaged = false;

        StopAllCoroutines();

        if (currentTroop != null)
        {
            Troops troop = currentTroop;
            currentTroop = null;

            troop.EnemyDied();
        }
    }

    public bool IsEngaged()
    {
        return engaged || attackingBarricade;
    }

    public bool IsDead()
    {
        return enemyHealth == null ||
               enemyHealth.IsDead();
    }
}
