using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayer
{
    public static PlayerController Instance { get; private set; }
    PlayerPowerupSystem powerupController;
    private float verticalInput;
    private Rigidbody playerRb;
    public float speed = 10; //3
    float baseSpeed = 10; //3
    float baseMass = 10;
    private GameObject focalPoint;
    public PowerupType currentPowerup { get; set; }

    private Coroutine powerUpCountdown;
    float powerupTime = 7;
    [SerializeField] private TextMeshProUGUI powerupCountdownText;
    float powerupRemaining;

    private PlayerActionsExample playerInput;
    private InputAction move;
    private InputAction castSkill;
    [SerializeField] public GameObject joystick;
    [SerializeField] public GameObject castButton;

    [SerializeField] PlayerSkinDB skinDB;

    public int point;
    List<IPlayer> touchedPlayer = new List<IPlayer>();

    private float lowerBound = -10;
    private float sideBound = 40;
    private float topBound = 10;

    public bool isPointGiven;

    [SerializeField] GameObject canvas;
    [SerializeField] GameObject nameTextPrefab;
    GameObject nameText;
    Vector3 nameOffset = new Vector3(0, 1, 0);
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }


        //player input 
        playerInput = new PlayerActionsExample();
        move = playerInput.Player.Move;
        move.Enable();
        castSkill = playerInput.Player.CastSkill;
        castSkill.Enable();
        castSkill.performed += CastSkill;

        SetStartStat();
        NoPowerup();
    }

    void SetStartStat()
    {
        playerRb = this.GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("Focal Point");

        this.GetComponent<MeshRenderer>().material = skinDB.GetSkin(PlayerHelper.Instance.skinID);
        if (PlayerHelper.Instance.skinID != 9)
        {
            this.GetComponent<MeshRenderer>().material.color = PlayerHelper.Instance.skinColor;
        }

        point = 0;
        isPointGiven = false;
        speed = baseSpeed * playerRb.mass;

        powerupCountdownText.gameObject.SetActive(false);
        //joystick.gameObject.SetActive(true);
        castButton.gameObject.SetActive(false);


        canvas = GameObject.Find("Canvas");

        nameText = Instantiate(nameTextPrefab, canvas.transform);
        nameText.GetComponent<TextMeshProUGUI>().text = PlayerHelper.Instance.playerName;
        nameText.GetComponent<TextMeshProUGUI>().color = Color.yellow;
    }

    void Update()
    {
        HandleMovement();
        HandlePowerup();
        DestroyOutOfBound();
    }
    void LateUpdate()
    {
        HandleNameText();
    }
    void HandleNameText()
    {
        nameText.transform.position = Camera.main.WorldToScreenPoint(this.transform.position + nameOffset);
    }

    void HandleMovement()
    {
        Vector2 input = move.ReadValue<Vector2>();
        verticalInput = input.y;
        if (isGrounded())
        {
            playerRb.AddForce(focalPoint.transform.forward * speed * verticalInput);
        }
    }

    void HandlePowerup()
    {
        DisplayTime(powerupRemaining);
        powerupRemaining -= Time.deltaTime;

        if (GameManager.Instance.isGameOver)
        {
            powerupCountdownText.gameObject.SetActive(false);
        }
    }

    private void CastSkill(InputAction.CallbackContext ctx)
    {
        if (powerupController != null)
        {
            ICastable iCast = powerupController.GetComponent<ICastable>();
            if (iCast != null)
                iCast?.Cast();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            NoPowerup();

            powerupCountdownText.gameObject.SetActive(true);

            currentPowerup = other.gameObject.GetComponent<Powerup>().powerupType;

            //Debug.Log(currentPowerup);
            switch (currentPowerup)
            {
                case PowerupType.Pushback:
                    powerupController = this.gameObject.AddComponent<Pushback>() as PlayerPowerupSystem;
                    break;
                case PowerupType.Rockets:
                    powerupController = this.gameObject.AddComponent<Rockets>() as PlayerPowerupSystem;
                    break;
                case PowerupType.Smash:
                    powerupController = this.gameObject.AddComponent<Smash>() as PlayerPowerupSystem;
                    break; ;
                case PowerupType.Dash:
                    powerupController = this.gameObject.AddComponent<Dash>() as PlayerPowerupSystem;
                    break;
                case PowerupType._1000tons:
                    powerupController = this.gameObject.AddComponent<_1000tons>() as PlayerPowerupSystem;
                    break;
            }

            powerupRemaining = powerupTime = powerupController.powerupTime;

            Destroy(other.gameObject);
            Debug.Log("Powerup with " + other.name);

            if (powerUpCountdown != null)
            {
                StopCoroutine(powerUpCountdown);
            }
            powerUpCountdown = StartCoroutine(PowerupCountdownRoutine());
        }
    }

    IEnumerator PowerupCountdownRoutine()
    {
        yield return new WaitForSeconds(powerupTime);
        NoPowerup();
        Debug.Log("Powerup countdown over");
    }

    public void NoPowerup()
    {
        if (powerupController != null)
        {
            Destroy(powerupController);
        }

        powerupCountdownText.gameObject.SetActive(false);

        currentPowerup = PowerupType.None;
        castButton.gameObject.SetActive(false);
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        powerupCountdownText.text = seconds.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlayCollsionWithEnemySfx(this.transform.position);
            SetTouchedPlayer(collision.gameObject.GetComponent<IPlayer>());
        }
    }

    void DestroyOutOfBound()
    {
        //destroy out of bound
        if ((transform.position.y < lowerBound) || (transform.position.x > sideBound) || (transform.position.z > sideBound) || (transform.position.y > topBound))
        {
            if (!isPointGiven)
            {
                isPointGiven = true;
                foreach (var player in touchedPlayer)
                {
                    MonoBehaviour mb = player as MonoBehaviour;
                    if (mb != null)
                        //if (player != null)
                        player?.AddPoint();
                }
                touchedPlayer.Clear();
            }
        }
    }
    public bool isGrounded()
    {
        RaycastHit hit;
        float distance = 2f;
        Vector3 dir = new Vector3(0, -2);

        if (Physics.Raycast(transform.position, dir, out hit, distance))
        {
            return true;
        }
        return false;
    }

    public void AddPoint()
    {
        this.point++;
        this.playerRb.mass = baseMass + point * 2;
        this.speed = baseSpeed * this.playerRb.mass;
        this.transform.localScale *= 1.1f;
    }

    public int GetPoint()
    {
        return this.point;
    }
    public void SetTouchedPlayer(IPlayer player)
    {
        StartCoroutine(TouchedPlayer(player));
    }
    IEnumerator TouchedPlayer(IPlayer player)
    {
        if (touchedPlayer.Contains(player))
            touchedPlayer.Remove(player); //reset if touch again
        touchedPlayer.Add(player);
       
        yield return new WaitForSeconds(7); //touch time >7s => no kill count
        touchedPlayer.Remove(player);
    }

    public void ResetPoint()
    {
        point = 0;
        playerRb.mass = 10;
        speed = baseSpeed * playerRb.mass;
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }
}

