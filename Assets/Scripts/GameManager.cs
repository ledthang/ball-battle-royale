using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject gameOverLayer;
    public Button playAgainButton;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI loadText;
    [SerializeField] GameObject loadingCanvas;
    public bool isGameOver { get; set; }
    public bool isWaveStartCountdown { get; set; }
    //public bool isWaveStart { get; set; }
    public float timeRemaining { get; set; }
    //public int waveNumber;

    private float lowerBound = -10;

    bool gameOverSoundPlayed = false;
    public Coroutine dropPlayerCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        isGameOver = false;
        //isWaveStartCountdown = true;
        //timeRemaining = 3;
        //============================
        player = GameObject.Find("Player") as GameObject;
        gameOverSoundPlayed = false;
    }

    void Start()
    {
        //AdsManager.Instance.ShowBannerAd();
        StartCoroutine(LoadAndStartGame());
    }

    IEnumerator LoadAndStartGame()
    {
        int loopCount = Random.Range(3, 7);
        for (int i = 0; i < loopCount; i++)
        {
            loadText.text = ".";
            yield return new WaitForSeconds(0.25f);
            loadText.text = "..";
            yield return new WaitForSeconds(0.25f);
            loadText.text = "...";
            yield return new WaitForSeconds(0.25f);
        }
        loopCount = Random.Range(3, 7);
        for (int i = 0; i < loopCount; i++)
        {
            loadText.text = "Loading.";
            yield return new WaitForSeconds(0.25f);
            loadText.text = "Loading..";
            yield return new WaitForSeconds(0.25f);
            loadText.text = "Loading...";
            yield return new WaitForSeconds(0.25f);
        }
        loadingCanvas.SetActive(false);
        PlayAgainButton();
    }

    void Update()
    {
        CheckGameOver();

        if (isGameOver)
        {
            if (!gameOverSoundPlayed)
            {
                gameOverSoundPlayed = true;
                AudioManager.Instance.PlayGameOver();
            }
            //player.gameObject.SetActive(false);
#if UNITY_ANDROID || UNITY_IPHONE
            PlayerController.Instance.joystick.SetActive(false);
            PlayerController.Instance.castButton.SetActive(false);
#endif
            timeText.gameObject.SetActive(false);
            gameOverLayer.gameObject.SetActive(true);
        }

        if (isWaveStartCountdown)
        {
            //Debug.Log("countdown");
            timeText.gameObject.SetActive(true);
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
            if (timeRemaining <= 0)
            {
                //Debug.Log("stop countdown");
                timeText.gameObject.SetActive(false);
                //isWaveStart = true;
                isWaveStartCountdown = false;
            }
        }
    }

    void CheckGameOver()
    {
        //now do in PlayerController
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = seconds.ToString();
    }

    public void PlayButtonClick()
    {
        AudioManager.Instance.PlayButtonClicked();
    }

    public void PlayAgainButton()
    {
        //Debug.Log("Start play again coroutine");
        gameOverLayer.gameObject.SetActive(false);
        isWaveStartCountdown = true;
        timeRemaining = 3;
        PlayerController.Instance.ResetPoint();
        isGameOver = false;
        player.SetActive(false);
        PlayerController.Instance.isPointGiven = false;

        EnemyBehaviour[] enemyLeft = FindObjectsOfType<EnemyBehaviour>();
        int enemyCount = enemyLeft.Length;

        //Debug.Log("Drop player");
        dropPlayerCoroutine = StartCoroutine(DropPlayer());
    }
    IEnumerator DropPlayer()
    {
        yield return new WaitForSeconds(3);
        player.transform.position = SpawnManager.Instance.GenerateSpawnPosition() + new Vector3(0, 3, 0);
        player.SetActive(true);

        Rigidbody rb = player.GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        PlayerController.Instance.NoPowerup();
#if UNITY_ANDROID || UNITY_IPHONE
        PlayerController.Instance.joystick.SetActive(true);
        PlayerController.Instance.castButton.SetActive(false);
#endif
    }
    public void MainMenu()
    {
        //AdsManager.Instance.HideBannerAd();
        SceneManager.LoadScene(0);
    }
}