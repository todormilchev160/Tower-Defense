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



    private Animator animator;


    private NavMeshAgent agent;
    private EnemyWalk enemyWalk;
    private EnemyHealth enemyHealth;


    // Troop combat
    private Troops currentTroop;

    private bool engaged = false;


    // Barricade combat
    private Barricade currentBarricade;

    private Coroutine barricadeAttackCoroutine;

    private bool attackingBarricade = false;


    // =====================================================
    // START
    // =====================================================

    void Start()
    {
        agent =
            GetComponent<NavMeshAgent>();

        enemyWalk =
            GetComponent<EnemyWalk>();

        enemyHealth =
            GetComponent<EnemyHealth>();
            animator=GetComponentInChildren<Animator>();
    }


    // =====================================================
    // UPDATE
    // =====================================================

    void Update()
    {
        if (IsDead())
            return;


        // =================================================
        // WE WERE ATTACKING A BARRICADE BUT IT WAS DESTROYED
        // =================================================

        if (
            attackingBarricade &&
            currentBarricade == null
        )
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

            return;
        }
    }


    // =====================================================
    // TROOP COMBAT
    // =====================================================

    public void Engage(Troops troop)
    {
        if (
            enemyHealth == null ||
            enemyHealth.IsDead()
        )
        {
            return;
        }


        if (engaged)
            return;


        if (attackingBarricade)
            return;


        if (troop == null)
            return;


        currentTroop =
            troop;


        engaged =
            true;


        agent.isStopped =
            true;


        agent.ResetPath();


        StartCoroutine(
            AttackRoutine()
        );
    }


    IEnumerator AttackRoutine()
    {
        while (engaged)
        {
            if (animator != null)
            {
                animator.SetBool(
                    "IsAttacking",
                    true
                );
            }


            yield return new WaitForSeconds(
                attackInterval
            );


            if (currentTroop == null)
                break;


            currentTroop.TakeDamage(
                damage
            );
        }
    }


    // =====================================================
    // TROOP DIED
    // =====================================================

    public void TroopDied(Troops troop)
    {
        if (troop != currentTroop)
            return;


        if (animator != null)
        {
            animator.SetBool(
                "IsAttacking",
                false
            );
        }


        currentTroop =
            null;


        engaged =
            false;


        StopAllCoroutines();


        if (
            enemyHealth != null &&
            !enemyHealth.IsDead()
        )
        {
            enemyWalk.ResumeMovement();
        }
    }

    void FindNearestBarricade()
    {
        // Already have one
        if (currentBarricade != null)
            return;


        Barricade[] barricades =
            FindObjectsByType<Barricade>(
                FindObjectsSortMode.None
            );


        float closestDistance =
            Mathf.Infinity;


        Barricade closestBarricade =
            null;


        foreach (Barricade barricade in barricades)
        {
            if (barricade == null)
                continue;


            if (barricade.IsDead())
                continue;


            float distance =
                Vector3.Distance(
                    transform.position,
                    barricade.transform.position
                );


            if (
                distance >
                barricadeDetectionRange
            )
            {
                continue;
            }


            if (distance < closestDistance)
            {
                closestDistance =
                    distance;


                closestBarricade =
                    barricade;
            }
        }


        currentBarricade =
            closestBarricade;
    }


    // =====================================================
    // HANDLE BARRICADE
    // =====================================================

    void HandleBarricade()
    {
        if (currentBarricade == null)
        {
            if (attackingBarricade)
            {
                FinishBarricadeAttack();
            }

            return;
        }


        if (currentBarricade.IsDead())
        {
            FinishBarricadeAttack();

            return;
        }


        float distance =
            Vector3.Distance(
                transform.position,
                currentBarricade.transform.position
            );


        // =================================================
        // MOVE TOWARD BARRICADE
        // =================================================

        if (
            distance >
            barricadeAttackDistance
        )
        {
            agent.isStopped =
                false;


            agent.SetDestination(
                currentBarricade.transform.position
            );


            return;
        }

        agent.isStopped =
            true;


        agent.ResetPath();


        attackingBarricade =
            true;


        if (animator != null)
        {
            animator.SetBool(
                "IsAttacking",
                true
            );
        }


        if (
            barricadeAttackCoroutine ==
            null
        )
        {
            barricadeAttackCoroutine =
                StartCoroutine(
                    AttackBarricadeRoutine()
                );
        }
    }


    // =====================================================
    // ATTACK BARRICADE
    // =====================================================

    IEnumerator AttackBarricadeRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                barricadeAttackInterval
            );


            // Barricade was destroyed
            if (currentBarricade == null)
            {
                FinishBarricadeAttack();

                yield break;
            }


            if (currentBarricade.IsDead())
            {
                FinishBarricadeAttack();

                yield break;
            }


            currentBarricade.TakeDamage(
                barricadeDamage
            );


            // Our attack may have killed it
            if (
                currentBarricade == null ||
                currentBarricade.IsDead()
            )
            {
                FinishBarricadeAttack();

                yield break;
            }
        }
    }

    void FinishBarricadeAttack()
    {
        currentBarricade =
            null;


        barricadeAttackCoroutine =
            null;


        attackingBarricade =
            false;


        if (animator != null)
        {
            animator.SetBool(
                "IsAttacking",
                false
            );
        }
        if (
            enemyHealth != null &&
            !enemyHealth.IsDead()
        )
        {
            enemyWalk.ResumeMovement();
        }
    }

    public void Die()
    {
        attackingBarricade =
            false;


        currentBarricade =
            null;


        barricadeAttackCoroutine =
            null;


        if (currentTroop != null)
        {
            Troops troop =
                currentTroop;


            currentTroop =
                null;


            engaged =
                false;


            StopAllCoroutines();


            troop.EnemyDied();
        }
        else
        {
            StopAllCoroutines();
        }
    }

    public bool IsEngaged()
    {
        return engaged;
    }


    public bool IsDead()
    {
        return
            enemyHealth == null ||
            enemyHealth.IsDead();
    }
}