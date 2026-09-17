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

    private bool towerUIOpen = false;
    private bool firstStage=true;
    private bool secondStage=false;
    private bool archer=false;
    private bool magic=false;
    private bool bomb=false;
    private GameObject tower;

    void Start()
    {
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
        magic=true;
        SpawnTower(magicTowerPrefab);
    }

    public void SpawnArcherTower()
    {
        archer=true;
        SpawnTower(archerTowerPrefab);
    }

    public void SpawnBombTower()
    {
        bomb=true;
        SpawnTower(bombTowerPrefab);
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
      Destroy(tower);
      firstStage=true;
      secondStage=false;
      slotButton.SetActive(true);
      towerUI.SetActive(false);

    }
    public void UpgradeTower()
    {
        if(archer)
        {
            UpgradeArcherTower();
        }
        if(magic)
        {
            UpgradeMagicTower();
        }
        if(bomb)
        {
            UpgradeBombTower();
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