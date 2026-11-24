using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;
public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        [Header("Enemies In This Wave")]
        public GameObject[] enemies;

        [Header("Spawn Settings")]
        public float timeBetweenEnemies = 1f;
    }

    [Header("Waves")]
    public Wave[] waves;

    [Header("Wave Settings")]
    public float timeBetweenWaves = 100f;

    [Header("Spawn Point")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;

    [Header("UI elements")]
    public Image clock;
    public TextMeshProUGUI wavecounter;
    public GameObject waveGrayedOut;

    [Header("Object To Disable")]
    [SerializeField] private GameObject objectToDisable;

    public static float clocktime;
    private float maxClockTime;
    private int currentWave = 0;
    private bool clockrunout = false;
    private bool clockticking = false;
    private int wavenumber = 1;
    private int maxwaves;
    

    void Start()
    {
        maxwaves=waves.Length;
        clocktime = timeBetweenWaves;
        maxClockTime = timeBetweenWaves;
        StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    { 
        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }
        while (currentWave < waves.Length)
        {
            waveGrayedOut.SetActive(false);
            if (currentWave != 0)
            {
                clockticking = true;
                yield return new WaitUntil(() => clockrunout);
                clockticking = false;
            }

            yield return StartCoroutine(
                SpawnWave(waves[currentWave])
            );

            currentWave++;
            wavenumber++;
        }
    }

    IEnumerator SpawnWave(Wave wave)
    {
        waveGrayedOut.SetActive(true);
        foreach (GameObject enemy in wave.enemies)
        {
            if (enemy != null)
            {
                float randomX = Random.Range(
                    leftSpawnPoint.position.x,
                    rightSpawnPoint.position.x
                );

                Vector3 spawnPosition = new Vector3(
                    randomX,
                    leftSpawnPoint.position.y,
                    leftSpawnPoint.position.z
                );

                Instantiate(
                    enemy,
                    spawnPosition,
                    leftSpawnPoint.rotation
                );
            }

            yield return new WaitForSeconds(wave.timeBetweenEnemies);
        }
    }

    void Update()
    {
        wavecounter.text ="Wave " +wavenumber+"/"+maxwaves;
        clock.fillAmount = clocktime / maxClockTime;

        if (clocktime == 0)
        {
            clockrunout = true;
        }
        else
        {
            clockrunout = false;
        }

        if (clockticking)
        {
            clocktime--;
        }
        else
        {
            clocktime = maxClockTime;
        }
    }
}