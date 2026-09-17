using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TowerUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject towerSelectionUI;
    public GameObject slotButton;
    public GameObject towerOptions;
    public GameObject towerUI;

    [Header("Tower Prefabs")]
    public GameObject archerTowerPrefab;
    public GameObject magicTowerPrefab;
    public GameObject bombTowerPrefab;
    public GameObject upgradedArcherPrefab;
    public GameObject upgradedMagicPrefab;
    public GameObject upgradedBombPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnY = 0f;
    [Header("Prices")]
    public int archerPrice;
    public int magicPrice;
    public int bombPrice;
    public int archerUpgradePrice;
    public int bombUpgradePrice;
    public int magicUpgradePrice;
    public int archerSellPrice=80;
    public int bombSellPrice=240;
    public int magicSellPrice=160;
    private int archerSellPrice2;
    private int magicSellPrice2;
    private int bombSellPrice2;

    private bool towerUIOpen = false;
    private bool firstStage=true;
    private bool secondStage=false;
    private bool archer=false;
    private bool magic=false;
    private bool bomb=false;
    private GameObject tower;

    void Start()
    {
        archerSellPrice2=archerSellPrice;
        magicSellPrice2=magicSellPrice;
        bombSellPrice2=bombSellPrice;
        towerSelectionUI.SetActive(false);
        slotButton.SetActive(true);
    }

    void Update()
    {
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
            return;
        }
        else
        {
            GameManager.currency-=magicPrice;
             magic=true;
            SpawnTower(magicTowerPrefab);
        }
    }

    public void SpawnArcherTower()
    {
        if(GameManager.currency<archerPrice)
        {
            return;
        }
        else
        {
            GameManager.currency-=archerPrice;
            archer=true;
            SpawnTower(archerTowerPrefab);
        }
    }

    public void SpawnBombTower()
    {
        if(GameManager.currency<bombPrice)
        {
            return;
        }
        else
        {
            GameManager.currency-=bombPrice;
            bomb=true;
            SpawnTower(bombTowerPrefab);
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
    }
    public void SellTower()
    {
      if(magic)
        {
            GameManager.currency+=magicSellPrice2;
            magicSellPrice2=magicSellPrice;
        }
      else if(archer)
        {
            GameManager.currency+=archerSellPrice2;
            archerSellPrice2=archerSellPrice;
        }
        if(bomb)
        {
            GameManager.currency+=bombSellPrice2;
            bombSellPrice2=bombSellPrice;
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
            if(GameManager.currency<archerUpgradePrice)
            {
                return;
            }
            else
            {
                archerSellPrice2+=archerUpgradePrice*8/10;
                GameManager.currency-=archerUpgradePrice;
                UpgradeArcherTower();
            }
            
        }
        if(magic)
        {
            if(GameManager.currency<magicUpgradePrice)
            {
                return;
            }
            else
            {
                magicSellPrice2+=magicUpgradePrice*8/10;
                GameManager.currency-=magicUpgradePrice;
                UpgradeMagicTower();
            }
        }
        if(bomb)
        {
            if(GameManager.currency < bombUpgradePrice)
            {
                return;
            }
            else
            {
                bombSellPrice2+=bombUpgradePrice*8/10;
                GameManager.currency -=bombUpgradePrice;
                UpgradeBombTower();
            }
        }
    }
    private void UpgradeArcherTower()
    {
        Destroy(tower);
        SpawnTower(upgradedArcherPrefab);
    }
    private void UpgradeMagicTower()
    {
        Destroy(tower);
        SpawnTower(upgradedMagicPrefab);
    }
    private void UpgradeBombTower()
    {
        Destroy(tower);
        SpawnTower(upgradedBombPrefab);
    }
}