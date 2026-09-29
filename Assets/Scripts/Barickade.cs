using UnityEngine;
using UnityEngine.UI;

public class Barricade : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private Image healthbarFill;

    private float health;
    private bool isDead = false;


    void Start()
    {
        health = maxHealth;

        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;


        health -= damage;

        health = Mathf.Clamp(
            health,
            0f,
            maxHealth
        );


        UpdateHealthBar();


        if (health <= 0f)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthbarFill == null)
            return;


        healthbarFill.fillAmount =
            health / maxHealth;
    }

    void Die()
    {
        if (isDead)
            return;


        isDead = true;


        Destroy(gameObject);
    }


    public bool IsDead()
    {
        return isDead;
    }
}