using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour
{
    [Header("Spawn Settings")]
     [SerializeField] private float extraSpawnDelay = 0f;

public float ExtraSpawnDelay => extraSpawnDelay;
    [SerializeField] private float maxHealth = 10;
    private float health;
    [SerializeField] private bool isLastEnemy;
    [SerializeField] private int amountOfMoney;
    public Image healthbarFill;
    public TextMeshProUGUI damageText;
    public float damagefeedbacktime = 1;
    public Animator animator;
    public string victoryScene;
    public bool orc;
    private bool isDead = false;
    void Start()
    {
        health = maxHealth;
    }

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

    public void Die()
    {
        if (isLastEnemy)
        {
            SceneManager.LoadScene(victoryScene);
        }
        animator.SetTrigger("Die");
        GameManager.currency += amountOfMoney;

        isDead = true;

        if (isLastEnemy)
        {
            GameManager.waveCleared = true;
        }
        EnemyCombat combat = GetComponent<EnemyCombat>();

        if (combat != null)
        {
            combat.Die();
        }

        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }
    void Update()
    {
        healthbarFill.fillAmount = health / maxHealth;
    }
}