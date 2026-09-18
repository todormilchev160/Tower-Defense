using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float health = 10;
    [SerializeField] private bool isLastEnemy;
    [SerializeField] private int amountOfMoney;

    private bool isDead = false;

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.currency+=amountOfMoney;
        
        isDead = true;

        if (isLastEnemy)
        {
            GameManager.waveCleared = true;
        }

        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }
}