using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour {
    private bool canSummon = false;

    public GameObject defaultZombie;
    public GameObject toughZombie;

    public List<Transform> spawnPoints;
    public float spawnInterval = 0.15f;
    public float waveDelay = 20f;

    public int currentWave = 0;
    private bool isSpawning = false;
    
    public GameManager gameManager;

    public void Start() {
        currentWave = 0;
        isSpawning = false;
    }

    public void Reset() {
        currentWave = 0;
        isSpawning = false;
        canSummon = false;
        
        StopAllCoroutines();
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject zombie in zombies) {
            Destroy(zombie);
        }
    }

    public void StartSummon() {
        StartCoroutine(StartNextWave());
    }
    
    IEnumerator StartNextWave()
    {
        while (true) {
            if (canSummon) {
                currentWave++;

                int enemyCount = CalculateEnemyCount(currentWave);
                float strongChance = CalculateStrongChance(currentWave);

                yield return StartCoroutine(SpawnWave(enemyCount, strongChance));
                yield return new WaitForSeconds(waveDelay);
            }
        }
    }

    public IEnumerator SpawnWave(int enemycount, float strongChance) {
        isSpawning = true;
        for (int i = 0; i < enemycount; i++) {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            bool spawnStrong = Random.value < strongChance;

            GameObject zombieToSpawn;
            if (spawnStrong) {
                zombieToSpawn = toughZombie;
            }
            else {
                zombieToSpawn = defaultZombie;
            }
            
            GameObject zombie = Instantiate(zombieToSpawn, spawnPoint.position, Quaternion.identity);
            ZombieController controller = zombie.GetComponent<ZombieController>();
            ZombieAttack attack = zombie.GetComponent<ZombieAttack>();
            if (controller != null) {
                controller.gameManager = gameManager;
                controller.setCanMove(true);
            }
            if (attack != null) attack.setCanMove(true);
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    int CalculateEnemyCount(int waveNum)
    {
        int baseEnemies = 5;
        int scalingFactor = 3;
        return baseEnemies + (waveNum * scalingFactor);
    }

    float CalculateStrongChance(int waveNum)
    {
        float percent = waveNum * 0.03f;
        return Mathf.Clamp(percent, 0f, 0.4f);
    }

    public void setCanSummon(bool canSummon) {
        this.canSummon = canSummon;
    }
}
