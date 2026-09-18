using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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
    public float timeBetweenWaves = 5f;

    [Header("Spawn Point")]
    public Transform leftSpawnPoint;
    public Transform rightSpawnPoint;
    [Header("ClockVisualisation")]
    public Image clock;
    private float clocktime;
    private float maxClockTime;
    private int currentWave = 0;

    void Start()
    {
        clocktime=timeBetweenWaves;
        maxClockTime=timeBetweenWaves;
        StartCoroutine(StartWaves());
    }

    IEnumerator StartWaves()
    {
        while (currentWave < waves.Length)
        {
            yield return new WaitUntil(() => GameManager.waveCleared);
            clocktime--;
            yield return new WaitForSeconds(timeBetweenWaves);
            clocktime=maxClockTime;


            yield return StartCoroutine(
                SpawnWave(waves[currentWave])
            );
            currentWave++;
            
        }
    }

IEnumerator SpawnWave(Wave wave)
{
    GameManager.waveCleared = false;

    foreach (GameObject enemy in wave.enemies)
    {
        if (enemy != null)
        {
            // Random X between the two points
            float randomX = Random.Range(
                leftSpawnPoint.position.x,
                rightSpawnPoint.position.x
            );

            // Random X, fixed Y and Z
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
        clock.fillAmount=clocktime/maxClockTime;
    }
}