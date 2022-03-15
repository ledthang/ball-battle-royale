using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    GameObject player;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] EnemyDB enemyDB;
    [SerializeField] GameObject[] powerupPrefabs;

    private int bossRound = 10;
    private float bossScaleRatio = 1.5f;
    private float bossMassRatio = 3;
    private float bossSpeedRatio = 1.5f;
    private int miniEnemyToSpawn = 3;

    private float minienemyScaleRatio = 0.75f;
    private float minienemyMassRatio = 0.5f;
    private float minienemySpeedRatio = 0.25f;

    private float startRangeX = 9;
    private float startRangeZ = 9;

    private int enemyCount;
    private int maxEnemy = 27;
    private int maxBoss = 7;

    private int randomPowerup;
    private float powerupSpawnTime = 10;
    private Coroutine spawnPowerupCoroutine;

    int waveSeries;
    bool isFirstWave = true;

    public bool isAdsOpening = false;

    public enum EnemyType
    {
        Normal,
        Boss,
        MiniEnemy
    }
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        player = GameObject.Find("Player");

        Debug.Log("Enemy count : " + enemyDB.enemiesCount);

        Debug.Log("Wave series : " + Mathf.Min(GameManager.Instance.waveNumber / bossRound, enemyDB.enemiesCount));

        //SpawnPowerup();
        //SpawnEnemyWave(GameManager.Instance.waveNumber);
    }

    void Update()
    {
        if (isFirstWave)
        {
            waveSeries = Mathf.Min(GameManager.Instance.waveNumber / bossRound, enemyDB.enemiesCount);
            if (GameManager.Instance.waveNumber % bossRound == 0)
            {
                SpawnBossWave(GameManager.Instance.waveNumber);
            }
            else
            {
                SpawnEnemyWave(GameManager.Instance.waveNumber);
            }
            isFirstWave = false;
            SpawnPowerup();
        }
        else if (!GameManager.Instance.isGameOver)
        {
            enemyCount = FindObjectsOfType<EnemyBehaviour>().Length;
            if (enemyCount == 0)
            {
                if (GameManager.Instance.waveNumber > 50 || Random.Range(0, 3) == 0)
                //if (!interstitialAdShowed)
                {
                    //interstitialAdShowed = true;
                    AdsManager.Instance.ShowInterstitialAd();
                }

                if (GameManager.Instance.dropPlayerCoroutine != null)
                {
                    StopCoroutine(GameManager.Instance.dropPlayerCoroutine);
                }
                ResetStat();
                GameManager.Instance.waveNumber++;
                Debug.Log("spawn enemy wave :" + GameManager.Instance.waveNumber);
                waveSeries = Mathf.Min(GameManager.Instance.waveNumber / bossRound, enemyDB.enemiesCount);
                Debug.Log("wave series :" + waveSeries);
                //if boss round or not
                if (GameManager.Instance.waveNumber % bossRound == 0)
                {
                    SpawnBossWave(GameManager.Instance.waveNumber);
                }
                else
                {
                    SpawnEnemyWave(GameManager.Instance.waveNumber);
                }
            }

            SpawnPowerup();
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float x, z;
        do
        {
            x = Random.Range(-startRangeX, startRangeX);
        } while (Mathf.Abs(x) < 1);

        do
        {
            z = Random.Range(-startRangeZ, startRangeZ); ;
        } while (Mathf.Abs(z) < 1);

        return new Vector3(x, 0, z);
    }

    void SpawnEnemyWave(int waveNumber)
    {
        //for (int i = 0; i < enemyNum; i++)
        if (waveNumber <= (enemyDB.enemiesCount) * bossRound)
        {
            CreateNewEnemy(enemyDB, waveSeries, EnemyType.Normal); //alway have 1 type in wave series

            for (int i = 1; i <= waveNumber % bossRound; i++)
            {
                int enemyIndex = Random.Range(0, waveSeries + 1);

                CreateNewEnemy(enemyDB, enemyIndex, EnemyType.Normal);
            }
        }
        else
        {
            int enemyToSpawn = (waveNumber - enemyDB.enemiesCount * bossRound) % maxEnemy;
            for (int i = 0; i <= enemyToSpawn; i++)
            {
                int enemyIndex = Random.Range(0, enemyDB.enemiesCount);

                CreateNewEnemy(enemyDB, enemyIndex, EnemyType.Normal);
            }
        }
    }

    public void Create1Enemy()
    {
        int enemyID;
        if (GameManager.Instance.waveNumber > (enemyDB.enemiesCount * bossRound))
        {
            enemyID = Random.Range(0, enemyDB.enemiesCount);
        }
        else enemyID = waveSeries;
        CreateNewEnemy(enemyDB, enemyID, EnemyType.Normal);
    }

    void CreateNewEnemy(EnemyDB DB, int ID, EnemyType type)
    {
        Enemy current = DB.enemies[ID]; //get current enemy info from DB

        GameObject newEnemy = Instantiate(enemyPrefab, GenerateSpawnPosition(), enemyPrefab.transform.rotation) as GameObject;
        newEnemy.name += current.description;

        Rigidbody rb = newEnemy.GetComponent<Rigidbody>();
        rb.mass = (newEnemy.GetComponent<Rigidbody>().mass + GameManager.Instance.waveNumber / 2) * current.massRatio;

        EnemyBehaviour eb = newEnemy.GetComponent<EnemyBehaviour>();
        eb.speed *= current.speedRatio;
        eb.specialAbility = current.specialAbility;

        MeshRenderer mr = newEnemy.GetComponent<MeshRenderer>();
        mr.material = current.material;
        mr.material.color = current.color;


        newEnemy.transform.localScale *= current.scaleRatio;
        eb.isBoss = false;
        eb.spawnInterval = current.spawnInterval;

        switch (type)
        {
            case EnemyType.Normal:
                break;
            case EnemyType.Boss:
                newEnemy.name = "BOSS " + newEnemy.name;
                rb.mass *= bossMassRatio;
                newEnemy.transform.localScale *= bossScaleRatio;
                eb.speed *= bossSpeedRatio;
                eb.isBoss = true;
                eb.spawnInterval = 3;
                eb.miniEnemySpawnCount = miniEnemyToSpawn;
                break;
            case EnemyType.MiniEnemy:
                newEnemy.name = "Mini enemy " + newEnemy.name;
                rb.mass *= minienemyMassRatio;
                newEnemy.transform.localScale *= minienemyScaleRatio;
                eb.speed *= minienemySpeedRatio;
                break;
        }
    }

    void SpawnPowerup()
    {
        if (spawnPowerupCoroutine == null)
        {
            spawnPowerupCoroutine = StartCoroutine(_SpawnPowerup());
        }
    }

    IEnumerator _SpawnPowerup()
    {
        int powerupRange = GameManager.Instance.waveNumber switch
        {
            var n when n <= 10 => 1,
            var n when n > 10 && n <= 20 => 2,
            var n when n > 20 && n <= 30 => 3,
            var n when n > 30 && n <= 40 => 4,
            var n when n > 40 && n <= 50 => 5,
            _ => 5
        };
        randomPowerup = Random.Range(0, powerupRange);
        Debug.Log("Total " + powerupPrefabs.Length + ", current " + powerupRange);
        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation);

        yield return new WaitForSeconds(powerupSpawnTime);

        spawnPowerupCoroutine = null;
    }

    void ResetStat()
    {
        //reset position
        player.gameObject.SetActive(true);
        player.transform.position = Vector3.zero;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        //nomore powerup
        PlayerController.Instance.NoPowerup();
        //PlayerController.Instance.powerupIndicator.SetActive(false);
        PlayerController.Instance.joystick.SetActive(true);

        //reset game manager
        GameManager.Instance.isWaveStartCountdown = true;
        GameManager.Instance.isWaveStart = false;
        GameManager.Instance.timeRemaining = 3;
    }

    void SpawnBossWave(int currentRound)
    {
        if (currentRound <= (enemyDB.enemiesCount * bossRound))
        {
            Debug.Log("Create 1 boss ID " + (waveSeries - 1));
            miniEnemyToSpawn = 3;
            CreateNewEnemy(enemyDB, waveSeries - 1, EnemyType.Boss);
        }
        else
        {
            int bossToSpawn = (currentRound / (bossRound * enemyDB.enemiesCount) + 1);
            if (bossToSpawn > maxBoss) bossToSpawn = maxBoss;
            for (int i = 0; i < bossToSpawn; i++)
            {
                int randomIndex = Random.Range(0, enemyDB.enemiesCount);
                miniEnemyToSpawn = 3 + (currentRound / bossRound - enemyDB.enemiesCount) % enemyDB.enemiesCount;
                Debug.Log("Create " + i + " boss ID " + randomIndex + " with " + miniEnemyToSpawn + "mini enemies");
                CreateNewEnemy(enemyDB, randomIndex, EnemyType.Boss);
            }
        }
    }

    public void SpawnMiniEnemy(int amount)
    {
        if (!GameManager.Instance.isGameOver)
            for (int i = 0; i < amount; i++)
            {
                int randomIndex = Random.Range(0, waveSeries);
                CreateNewEnemy(enemyDB, randomIndex, EnemyType.MiniEnemy);
            }
    }
}