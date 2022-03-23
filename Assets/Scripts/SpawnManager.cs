using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    GameObject player;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject[] powerupPrefabs;

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject nameTextPrefab;

    private float startRangeX = 18;
    private float startRangeZ = 18;
    private Vector3 spawnPlayerOffset = new Vector3(0, 3, 0);

    public int enemyCount;
    private int maxEnemy = 20;
    bool isStartWave;

    private int randomPowerup;
    private float powerupSpawnTime = 5;
    private Coroutine spawnPowerupCoroutine;

    string[] names;

    private Coroutine spawnARandomEnemy;

    [SerializeField] PlayerSkinDB skinDB;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        TextAsset nameData = Resources.Load("names") as TextAsset;
        var content = nameData.text;
        //names = File.ReadAllLines("Assets/Resources/names.json");
        names = content.Split('\n');
        enemyCount = 1;
        isStartWave = true;
        SpawnEnemyWave(Random.Range(7, maxEnemy));
        isStartWave = false;
    }

    void Update()
    {
        SpawnPowerup();

        StartCoroutine(SpawnAEnemy());
    }

    public Vector3 GenerateSpawnPosition()
    {
        float x, z;
        x = Random.Range(-startRangeX, startRangeX);
        z = Random.Range(-startRangeZ, startRangeZ);

        return new Vector3(x, 0, z);
    }

    void SpawnEnemyWave(int waveNumber)
    {
        for (int i = 1; i <= waveNumber; i++)
        {
            CreateNewEnemy((TypeOfEnemy)Random.Range(0, 3));
            //CreateNewEnemy((TypeOfEnemy)2);
        }
    }

    void CreateNewEnemy(TypeOfEnemy type)
    {

        GameObject newEnemy = Instantiate(enemyPrefab, GenerateSpawnPosition() + spawnPlayerOffset, enemyPrefab.transform.rotation) as GameObject;
        newEnemy.name = names[Random.Range(0, names.Length)];

        Rigidbody rb = newEnemy.GetComponent<Rigidbody>();

        EnemyBehaviour eb = newEnemy.GetComponent<EnemyBehaviour>();
        eb.nameText = Instantiate(nameTextPrefab, canvas.transform);
        float r = Random.value;
        if (isStartWave)
            eb.point = r switch
            {
                var n when n > 0.95f => Random.Range(10, 20),
                var n when n > 0.80f => Random.Range(5, 10),
                var n when n > 0.50f => Random.Range(0, 5),
                _ => 0
            };
        else eb.point = 0;

        MeshRenderer mr = newEnemy.GetComponent<MeshRenderer>();
        mr.material = skinDB.GetSkin(Random.Range(0, skinDB.skinsCount));
        mr.material.color = RandomColor();

        switch (type)
        {
            case TypeOfEnemy.Normal:
                eb.typeOfEnemy = TypeOfEnemy.Normal;
                //mr.material.color = Color.clear;
                break;
            case TypeOfEnemy.Runaway:
                eb.typeOfEnemy = TypeOfEnemy.Runaway;
                //mr.material.color = Color.red;
                break;
            case TypeOfEnemy.Smart:
                eb.typeOfEnemy = TypeOfEnemy.Smart;
                //mr.material.color = Color.blue;
                break;
        }
    }

    IEnumerator SpawnAEnemy()
    {
        while (true)
        {
            //Debug.Log(enemyCount);
            float timeToNextSpawm = Random.Range(1, 10f);
            yield return new WaitForSeconds(timeToNextSpawm);
            if (enemyCount < maxEnemy)
            {
                CreateNewEnemy((TypeOfEnemy)Random.Range(0, 3));
                //CreateNewEnemy((TypeOfEnemy)2);
            }
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
        randomPowerup = Random.Range(0, 5);

        Instantiate(powerupPrefabs[randomPowerup], GenerateSpawnPosition(), powerupPrefabs[randomPowerup].transform.rotation);

        yield return new WaitForSeconds(powerupSpawnTime);

        spawnPowerupCoroutine = null;
    }

    Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}