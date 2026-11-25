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
    public GameObject towerUI2;
    public GameObject rallyPointButton;
    public GameObject rallyPointButton2;   [Header("Tower Prefabs")]
    public GameObject[] archerTowers;
    public GameObject[] magicTowers;
    public GameObject[] troopTowers;
    public GameObject[] bombTowers;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnY = 0f;
    [Header("Buy Prices")]
    public int archerPrice;
    public int magicPrice;
    public int bombPrice;
    public int troopPrice = 0;
    [Header("Upgrade prices and settings")]
    public int archerUpgradePrice;
    public int bombUpgradePrice;
    public int magicUpgradePrice;
    public int troopUpgradePrice = 0;
    public int maxLevel=3;
    public int currentMapLevel;
    public GameObject coin;
    [Header("Barrack settings")]
    public int maxbarracks = 1;
    [Header("Sell prices")]
    public int archerSellPrice = 80;
    public int bombSellPrice = 240;
    public int troopSellPrice = 0;
    public int magicSellPrice = 160;
    public int priceIncreaseAfterUgrade = 50;
    private int archerSellPrice2;
    private int magicSellPrice2;
    private int bombSellPrice2;
    private int troopSellPrice2;
    private int archerUpgradePrice2;
    private int bombUpgradePrice2;
    private int magicUpgradePrice2;
    private int troopUpgradePrice2;
    private int currentLevel = 0;

    private bool towerUIOpen = false;
    private bool firstStage = true;
    private bool secondStage = false;
    private bool archer = false;
    private bool magic = false;
    private bool bomb = false;
    private bool troop = false;
    private GameObject tower;
    private HoverActivation hoverActivation;
    public TextMeshProUGUI upgradePriceText;
    public TextMeshProUGUI sellPriceText;
    public TextMeshProUGUI archerPriceText;
    public TextMeshProUGUI magicPriceText;
    public TextMeshProUGUI bombPriceText;
    public TextMeshProUGUI troopPriceText;
    public GameObject cross;
    public static int currentbarrackscount = 0;
    private int level=1;

    void Start()
    {

        troopPriceText.text = "" + troopPrice;
        hoverActivation = GetComponentInParent<HoverActivation>();
        troopUpgradePrice2 = troopUpgradePrice;
        bombUpgradePrice2 = bombUpgradePrice;
        archerUpgradePrice2 = archerUpgradePrice;
        magicUpgradePrice2 = magicUpgradePrice;
        archerSellPrice2 = archerSellPrice;
        magicSellPrice2 = magicSellPrice;
        bombSellPrice2 = bombSellPrice;
        troopSellPrice2 = troopSellPrice;
        slotButton.SetActive(true);
        archerPriceText.text = "" + archerPrice;
        magicPriceText.text = "" + magicPrice;
        bombPriceText.text = "" + bombPrice;
    }

    void Update()
    {
        if(level==maxLevel)
        {
            coin.SetActive(false);
            upgradePriceText.text="Max";
        }
        else if (archer)
        {
            upgradePriceText.text = "" + archerUpgradePrice2;
            sellPriceText.text = "" + archerSellPrice2;
        }
        if (magic)
        {
            upgradePriceText.text = "" + magicUpgradePrice2;
            sellPriceText.text = "" + magicSellPrice2;
        }
        if (bomb)
        {
            upgradePriceText.text = "" + bombUpgradePrice2;
            sellPriceText.text = "" + bombSellPrice2;
        }
        if (troop)
        {
            upgradePriceText.text = "" + troopUpgradePrice2;
            sellPriceText.text = "" + troopSellPrice2;
        }

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
        HoverActivation.selecting = true;
        towerSelectionUI.SetActive(true);
        slotButton.SetActive(false);
        towerUIOpen = true;
    }

    
    public void CloseTowerUI()
{
    if (firstStage)
    {
        towerSelectionUI.SetActive(false);
        slotButton.SetActive(true);
    }

    if (secondStage)
    {
        towerUI.SetActive(false);
        towerOptions.SetActive(true);
    }

    towerUIOpen = false;
    

   
    if (hoverActivation != null)
        hoverActivation.UnlockHover();

    HoverActivation.selecting = false;
}

    public void SpawnMagicTower()
    {
        if (GameManager.currency < magicPrice)
        {
            StartCoroutine(NoMoneyTextCoroutine());
            return;
        }
        else
        {
            GameManager.currency -= magicPrice;
            magic = true;
            SpawnTower(magicTowers[0]);
            HoverActivation.selecting = false;
        }
    }

    public void SpawnArcherTower()
    {
        if (GameManager.currency < archerPrice)
        {
            StartCoroutine(NoMoneyTextCoroutine());
            return;
        }
        else
        {
            GameManager.currency -= archerPrice;
            archer = true;
            SpawnTower(archerTowers[0]);
            HoverActivation.selecting = false;
        }
    }

    public void SpawnBombTower()
    {
        if (GameManager.currency < bombPrice)
        {
            StartCoroutine(NoMoneyTextCoroutine());
            return;
        }
        else
        {
            GameManager.currency -= bombPrice;
            bomb = true;
            SpawnTower(bombTowers[0]);
            HoverActivation.selecting = false;
        }
    }
    public void SpawnBarracks()
    {

        if (GameManager.currency < troopPrice || currentbarrackscount == maxbarracks)
        {
            StartCoroutine(NoMoneyTextCoroutine());
            return;
        }
        else
        {
            currentbarrackscount += 1;
            GameManager.currency -= troopPrice;
            troop = true;
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

        tower = Instantiate(
            towerPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Assign this tower its original TowerSlot
        TowerHover towerHover = tower.GetComponent<TowerHover>();

        

        towerUI.SetActive(false);
        towerSelectionUI.SetActive(false);
        towerOptions.SetActive(true);

        firstStage = false;
        secondStage = true;
    }
public void ShowTowerOptions()
{
  
    if (hoverActivation != null)
        hoverActivation.LockHover();

    HoverActivation.selecting = true;
 
    towerOptions.SetActive(false);
    if(currentMapLevel==1)
    {
        towerUI2.SetActive(true);
    }
    else
    {
        towerUI.SetActive(true);
        }
      

    towerUIOpen = true;

    if (rallyPointButton != null)
        rallyPointButton.SetActive(troop);
}    public void SellTower()
    {
        level=1;
        coin.SetActive(true);
        currentLevel = 0;

        if (magic)
        {
            GameManager.currency += magicSellPrice2;
            magicSellPrice2 = magicSellPrice;
            magicUpgradePrice2 = magicUpgradePrice;
        }
        else if (archer)
        {
            GameManager.currency += archerSellPrice2;
            archerSellPrice2 = archerSellPrice;
            archerUpgradePrice2 = archerUpgradePrice;
        }
        if (bomb)
        {
            GameManager.currency += bombSellPrice2;
            bombSellPrice2 = bombSellPrice;
            bombUpgradePrice2 = bombUpgradePrice;
        }
        if (troop)
        {
            TroopRallyPoint.navMeshVisual.SetActive(false);
           currentbarrackscount --;
            GameManager.currency += troopSellPrice2;
            troopSellPrice2 = troopSellPrice;
            troopUpgradePrice2 = troopUpgradePrice;
        }
       if (hoverActivation != null)
    hoverActivation.UnlockHover();

towerUIOpen = false;


HoverActivation.selecting = false;
        Destroy(tower);
        firstStage = true;
        secondStage = false;
        slotButton.SetActive(true);
        towerUI.SetActive(false);
        bomb = false;
        magic = false;
        archer = false;

    }
    public void UpgradeTower()
    {
        if(level==maxLevel)
        {
            StartCoroutine(NoMoneyTextCoroutine());
                return;
        }
        if (archer)
        {
            if (GameManager.currency < archerUpgradePrice2)
            {
                StartCoroutine(NoMoneyTextCoroutine());
                return;
            }
            else
            {
                level++;
                HoverActivation.selecting = false;
                archerSellPrice2 += archerUpgradePrice * 8 / 10;
                GameManager.currency -= archerUpgradePrice2;
                archerUpgradePrice2 += priceIncreaseAfterUgrade;
                UpgradeArcherTower();
            }
        }
        if (magic)
        {
            if (GameManager.currency < magicUpgradePrice2)
            {
                StartCoroutine(NoMoneyTextCoroutine());
                return;
            }
            else
            {
                level++;
                HoverActivation.selecting = false;
                magicSellPrice2 += magicUpgradePrice * 8 / 10;
                GameManager.currency -= magicUpgradePrice2;
                magicUpgradePrice2 += priceIncreaseAfterUgrade;
                UpgradeMagicTower();
            }
        }
        if (bomb)
        {
            if (GameManager.currency < bombUpgradePrice2)
            {
                StartCoroutine(NoMoneyTextCoroutine());
                return;
            }
            else
            {
                level++;
                HoverActivation.selecting = false;
                bombSellPrice2 += bombUpgradePrice * 8 / 10;
                GameManager.currency -= bombUpgradePrice2;
                bombUpgradePrice2 += priceIncreaseAfterUgrade;
                UpgradeBombTower();
            }
        }
        if (troop)
        {
            if (GameManager.currency < troopUpgradePrice2)
            {
                StartCoroutine(NoMoneyTextCoroutine());
                return;
            }
            else
            {
                level++;
                HoverActivation.selecting = false;
                troopSellPrice2 += troopUpgradePrice * 8 / 10;
                GameManager.currency -= troopUpgradePrice2;
                troopUpgradePrice2 += priceIncreaseAfterUgrade;
                UpgradeBarracks();
            }
        }
    }
    private void UpgradeArcherTower()
    {

        Destroy(tower);
        currentLevel += 1;
        SpawnTower(archerTowers[currentLevel]);
    }
    private void UpgradeMagicTower()
    {
        Destroy(tower);
        currentLevel += 1;
        SpawnTower(magicTowers[currentLevel]);
    }
    private void UpgradeBombTower()
    {
        Destroy(tower);
        currentLevel += 1;
        SpawnTower(bombTowers[currentLevel]);
    }
    private void UpgradeBarracks()
    {
        TroopRallyPoint.navMeshVisual.SetActive(false);
        Destroy(tower);
        currentLevel += 1;
        SpawnTower(troopTowers[currentLevel]);
    }
    private IEnumerator NoMoneyTextCoroutine()
    {
        cross.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        cross.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        cross.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        cross.SetActive(false);
        yield return new WaitForSeconds(0.2f);
        cross.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        cross.SetActive(false);
    }
}