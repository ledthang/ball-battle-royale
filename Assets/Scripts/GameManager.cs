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
    [SerializeField] private TextMeshProUGUI waveText;
    public bool isGameOver { get; private set; }
    public bool isWaveStartCountdown { get; set; }
    public bool isWaveStart { get; set; }
    public float timeRemaining { get; set; }
    public int waveNumber;

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
        isWaveStartCountdown = true;
        timeRemaining = 3;
        waveNumber = PlayerHelper.Instance.currentWave;
        player = GameObject.Find("Player") as GameObject;
        gameOverSoundPlayed = false;
    }

    void Start()
    {
        AdsManager.Instance.ShowBannerAd();
    }

    void Update()
    {
        CheckGameOver();
        waveText.text = waveNumber.ToString();

        if (isGameOver)
        {
            if (!gameOverSoundPlayed)
            {
                gameOverSoundPlayed = true;
                AudioManager.Instance.PlayGameOver();
            }
            //player.gameObject.SetActive(false);
            PlayerController.Instance.joystick.gameObject.SetActive(false);
            PlayerController.Instance.castButton.gameObject.SetActive(false);
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
                isWaveStart = true;
                isWaveStartCountdown = false;
            }
        }

        if (waveNumber % 1 == 0)
        {
            PlayerHelper.Instance.currentWave = waveNumber;
            //Debug.Log("Save wave: " + waveNumber);
        }
    }

    void CheckGameOver()
    {
        if (player.gameObject.activeSelf)
            if ((player.transform.position.y < lowerBound) || PlayerController.Instance.standTooLong)
            {
                isGameOver = true;
                PlayerController.Instance.standTooLong = false;
            }
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
        Debug.Log("Start play again coroutine");
        gameOverLayer.gameObject.SetActive(false);
        isWaveStartCountdown = true;
        timeRemaining = 3;
        isGameOver = false;
        player.SetActive(false);
        //player.transform.position = new Vector3(0, 10, 0);

        EnemyBehaviour[] enemyLeft = FindObjectsOfType<EnemyBehaviour>();
        int enemyCount = enemyLeft.Length;
       
        if (enemyCount == 0) 
            SpawnManager.Instance.Create1Enemy();

        Debug.Log("Drop player");
        dropPlayerCoroutine = StartCoroutine(DropPlayer());
    }
    IEnumerator DropPlayer()
    {
        yield return new WaitForSeconds(3);
        player.transform.position = new Vector3(0, 10, 0);
        player.SetActive(true);

        Rigidbody rb = player.GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        PlayerController.Instance.NoPowerup();
        PlayerController.Instance.joystick.SetActive(true);
    }
    public void MainMenu()
    {
        AdsManager.Instance.HideBannerAd();
        SceneManager.LoadScene(0);
    }

    public void ShowPlayAgainRewardAds()
    {
        AdsManager.Instance.ShowPlayAgainVideoRewardAd();
    }
    public void EnablePlayAgainButton()
    {
        playAgainButton.interactable = true;
    }
    public void DisablePlayAgainButton()
    {
        playAgainButton.interactable = false;
    }
}