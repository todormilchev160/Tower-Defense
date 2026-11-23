using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class BaseHealth : MonoBehaviour
{
    public Image healthbar;
    private float baseHealth;
    public int amountOfHealthIncreasedOnUpgrade;
    [Header("Prices")]
    public int baseUgradePrice=100;
    public int upgradePriceIncreaseAfterUpgrade=50;
    public int moneyAfterDowngrade;
    [SerializeField]private float maxHealth=100;
    private EnemyDamageBase enemyDamageBase;
    private string deathScene;
    private EnemyHealth enemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseHealth=maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        healthbar.fillAmount=baseHealth/maxHealth;
    }
    void OnTriggerEnter(Collider other)
    {

        enemyDamageBase=other.GetComponent<EnemyDamageBase>();
        enemy=other.GetComponentInParent<EnemyHealth>();

        baseHealth-=enemyDamageBase.damageDealtOnBase;
          enemy.Die();
        if(baseHealth<=0)
        {
            SceneManager.LoadScene(deathScene);
        }
    }
    public void UpgradeBase()
    {
        if(GameManager.currency<baseUgradePrice)
        {
            return;
        }
        else
        {
            maxHealth+=amountOfHealthIncreasedOnUpgrade;
            baseHealth+=amountOfHealthIncreasedOnUpgrade;
            GameManager.currency-=baseUgradePrice;
        }
    }

}