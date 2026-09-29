using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;
public class TowerUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject towerSelectionUI;
    public GameObject slotButton;
    public GameObject towerOptions;
    public GameObject towerUI;
    public TextMeshProUGUI archerTowerText;
    public TextMeshProUGUI magicTowerText;
    public TextMeshProUGUI bombTowerText;
    public TextMeshProUGUI upgradeText;
    public TextMeshProUGUI sellText;
    public TextMeshProUGUI noMoneyText;
    public TextMeshProUGUI troopText;
    public GameObject rallyPointButton;


    [Header("Tower Prefabs")]
    public GameObject[] archerTowers;
    public GameObject[] magicTowers;
    public GameObject[] troopTowers;
    public GameObject[] bombTowers;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnY = 0f;
    [Header("Prices")]
    [Header("Buy Prices")]
    public int archerPrice;
    public int magicPrice;
    public int bombPrice;
    public int troopPrice=0;
    [Header("Upgrade prices")]
    public int archerUpgradePrice;
    public int bombUpgradePrice;
    public int magicUpgradePrice;
    public int troopUpgradePrice=0;
    [Header("Sell prices")]
    public int archerSellPrice=80;
    public int bombSellPrice=240;
    public int troopSellPrice=0;
    public int magicSellPrice=160;
    public int priceIncreaseAfterUgrade=50;
    private int archerSellPrice2;
    private int magicSellPrice2;
    private int bombSellPrice2;
    private int troopSellPrice2;
    private int archerUpgradePrice2;
    private int bombUpgradePrice2;
    private int magicUpgradePrice2;
    private int troopUpgradePrice2;
    private int currentLevel=0;

    private bool towerUIOpen = false;
    private bool firstStage=true;
    private bool secondStage=false;
    private bool archer=false;
    private bool magic=false;
    private bool bomb=false;
    private bool troop=false;
    private GameObject tower;

    void Start()
    {
        troopUpgradePrice2=troopUpgradePrice;
        bombUpgradePrice2=bombUpgradePrice;
        archerUpgradePrice2=archerUpgradePrice;
        magicUpgradePrice2=magicUpgradePrice;
        archerSellPrice2=archerSellPrice;
        magicSellPrice2=magicSellPrice;
        bombSellPrice2=bombSellPrice;
        troopSellPrice2=troopSellPrice;
        towerSelectionUI.SetActive(false);
        slotButton.SetActive(true);
    }

    void Update()
    {
        if(magic)
        {
            upgradeText.text="Upgrade"+magicUpgradePrice2;
            sellText.text="Sell"+magicSellPrice2;
        }
        if(archer)
        {
            upgradeText.text="Upgrade"+archerUpgradePrice2;
            sellText.text="Sell"+archerSellPrice2;
        }
        if(bomb)
        {
            upgradeText.text="Upgrade"+bombUpgradePrice2;
            sellText.text="Sell"+bombSellPrice2;
        }
        archerTowerText.text="Archer Tower"+archerPrice;
        magicTowerText.text ="Magic Tower"+magicPrice;
        bombTowerText.text ="Bomb Tower"+bombPrice;
        troopText.text="Barracks" + troopPrice;
        if (!towerUIOpen)
            return;
       
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            CloseTowerUI();
        }
    }
    public void ShowTowerUI()
    {
        towerSelectionUI.SetActive(true);
        slotButton.SetActive(false);

        towerUIOpen = true;
    }

    public void CloseTowerUI()
    {
        if(firstStage)
        {
        towerSelectionUI.SetActive(false);
        slotButton.SetActive(true);
        towerUIOpen = false;
        }
        if(secondStage)
        {      
        towerOptions.SetActive(true);
        towerUI.SetActive(false);
        towerUIOpen = false;
        }
    }

    public void SpawnMagicTower()
    {
        if(GameManager.currency<magicPrice)
        {
            StartCoroutine(NoMoneyTextCoroutine());
            return;
        }
        else
        {
            GameManager.currency-=magicPrice;
             magic=true;
            SpawnTower(magicTowers[0]);
        }
    }

    public void SpawnArcherTower()
    {
        if(GameManager.currency<archerPrice)
        {
            StartCoroutine(NoMoneyTextCoroutine());
            return;
        }
        else
        {
            GameManager.currency-=archerPrice;
            archer=true;
            SpawnTower(archerTowers[0]);
        }
    }

    public void SpawnBombTower()
    {
        if(GameManager.currency<bombPrice)
        {
            StartCoroutine(NoMoneyTextCoroutine());
            return;
        }
        else
        {
            GameManager.currency-=bombPrice;
            bomb=true;
            SpawnTower(bombTowers[0]);
        }
    }
    public void SpawnBarracks()
    {
                
        if(GameManager.currency<troopPrice)
        {
            StartCoroutine(NoMoneyTextCoroutine());
            return;
        }
        else
        {
            GameManager.currency-=troopPrice;
            troop=true;
            SpawnTower(troopTowers[0]);
        }
    }
    private void SpawnTower(GameObject towerPrefab)
    {
        if (towerPrefab == null || slotButton == null)
            return;

        Vector3 spawnPosition = new Vector3(
            slotButton.transform.position.x,
            spawnY,
            slotButton.transform.position.z
        );

        tower=Instantiate(
            towerPrefab,
            spawnPosition,
            Quaternion.identity
        );
        towerUI.SetActive(false);
        towerSelectionUI.SetActive(false);
        towerOptions.SetActive(true);
        firstStage=false;
        secondStage=true;
    }
    public void ShowTowerOptions()
    {
        towerUIOpen=true;
        towerOptions.SetActive(false);
        towerUI.SetActive(true);
        if(!troop)
        {
            rallyPointButton.SetActive(false);
        }
    }
    public void SellTower()
    {
        currentLevel=0;
        
      if(magic)
        {
            GameManager.currency+=magicSellPrice2;
            magicSellPrice2=magicSellPrice;
            magicUpgradePrice2=magicUpgradePrice;
        }
      else if(archer)
        {
            GameManager.currency+=archerSellPrice2;
            archerSellPrice2=archerSellPrice;
            archerUpgradePrice2=archerUpgradePrice;
        }
        if(bomb)
        {
            GameManager.currency+=bombSellPrice2;
            bombSellPrice2=bombSellPrice;
            bombUpgradePrice2=bombUpgradePrice;
        }
        if(troop)
        {
            GameManager.currency+=troopSellPrice2;
           troopSellPrice2=troopSellPrice;
            troopUpgradePrice2=troopUpgradePrice;
        }
      Destroy(tower);
      firstStage=true;
      secondStage=false;
      slotButton.SetActive(true);
      towerUI.SetActive(false);
      bomb=false;
      magic=false;
      archer=false;

    }
    public void UpgradeTower()
    {
        if(archer)
        {
            if(GameManager.currency<archerUpgradePrice2)
            {
                StartCoroutine(NoMoneyTextCoroutine());
                return;
            }
            else
            {
                archerSellPrice2+=archerUpgradePrice*8/10;
                GameManager.currency-=archerUpgradePrice2;
                archerUpgradePrice2+=priceIncreaseAfterUgrade;
                UpgradeArcherTower();
            } 
        }
        if(magic)
        {
            if(GameManager.currency<magicUpgradePrice2)
            {
                StartCoroutine(NoMoneyTextCoroutine());
                return;
            }
            else
            {
                magicSellPrice2+=magicUpgradePrice*8/10;
                GameManager.currency-=magicUpgradePrice2;
                magicUpgradePrice2+=priceIncreaseAfterUgrade;
                UpgradeMagicTower();
            }
        }
        if(bomb)
        {
            if(GameManager.currency < bombUpgradePrice2)
            {
                StartCoroutine(NoMoneyTextCoroutine());
                return;
            }
            else
            {
                bombSellPrice2+=bombUpgradePrice*8/10;
                GameManager.currency -=bombUpgradePrice2;
                bombUpgradePrice2+=priceIncreaseAfterUgrade;
                UpgradeBombTower();
            }
        }
        if(troop)
        {
            if(GameManager.currency < troopUpgradePrice2)
            {
                StartCoroutine(NoMoneyTextCoroutine());
                return;
            }
            else
            {
                troopSellPrice2+=troopUpgradePrice*8/10;
                GameManager.currency -=troopUpgradePrice2;
                troopUpgradePrice2+=priceIncreaseAfterUgrade;
                UpgradeBombTower();
            }
        }
    }
    private void UpgradeArcherTower()
    {
        
        Destroy(tower);
        currentLevel+=1;
        SpawnTower(archerTowers[currentLevel]);
    }
    private void UpgradeMagicTower()
    {
        Destroy(tower);
        currentLevel+=1;
        SpawnTower(magicTowers[currentLevel]);
    }
    private void UpgradeBombTower()
    {
        Destroy(tower);
        currentLevel+=1;
        SpawnTower(bombTowers[currentLevel]);
    }
    private IEnumerator NoMoneyTextCoroutine()
    {
        noMoneyText.text="Insuficient funds";
        yield return new WaitForSeconds(1);
        noMoneyText.text="";
    }
}