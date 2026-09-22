using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float damage = 2f;
    [SerializeField] private float attackInterval = 1f;

    public Animator animator;

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
        if (enemyHealth == null || enemyHealth.IsDead())
            return;

        if (engaged)
            return;

        if (troop == null)
            return;

        currentTroop = troop;
        engaged = true;

        agent.isStopped = true;
        agent.ResetPath();

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (engaged)
        {
            animator.SetBool("IsAttacking",true);
            yield return new WaitForSeconds(attackInterval);

            if (currentTroop == null)
                break;

            currentTroop.TakeDamage(damage);
        }
    }

    public void TroopDied(Troops troop)
    {
        
        if (troop != currentTroop)
            return;
        animator.SetBool("IsAttacking",false);
        currentTroop = null;
        engaged = false;

        StopAllCoroutines();

        if (enemyHealth != null && !enemyHealth.IsDead())
        {
            enemyWalk.ResumeMovement();
        }
    }

    public void Die()
    {
        if (currentTroop != null)
        {
            Troops troop = currentTroop;

            currentTroop = null;
            engaged = false;

            StopAllCoroutines();

            troop.EnemyDied();
        }
    }

    public bool IsEngaged()
    {
        return engaged;
    }

    public bool IsDead()
    {
        return enemyHealth == null || enemyHealth.IsDead();
    }
}