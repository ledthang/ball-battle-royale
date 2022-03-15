using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    PlayerPowerupSystem powerupController;
    private float verticalInput;
    private Rigidbody playerRb;
    public float speed;
    private float speedRatio = 5; //speed / mass ratio
    private GameObject focalPoint;
    public PowerupType currentPowerup { get; set; }

    public GameObject powerupIndicator;
    private MeshRenderer powerupIndicatorRenderer;
    [SerializeField] Material powerupIndicatorBaseMaterial;
    private Coroutine powerUpCountdown;
    float powerupTime = 7;
    [SerializeField] private TextMeshProUGUI powerupCountdownText;
    float powerupRemaining;

    public delegate void DoPowerupCast();
    public static event DoPowerupCast doPowerupCast;
    public delegate void DoPowerupPassive();
    public static event DoPowerupPassive doPowerupPassive;

    private bool standStilltimerStarted = false;
    private float standStilltimer = 0;
    private float maxStandTime = 7;
    public bool standTooLong = false;

    private PlayerActionsExample playerInput;
    private InputAction move;
    private InputAction castSkill;
    [SerializeField] public GameObject joystick;
    [SerializeField] public GameObject castButton;

    [SerializeField] PlayerSkinDB skinDB;
    [SerializeField] PlayerPowerupIndicatorDB indicatorDB;

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

        Debug.Log("START GAME WITH: skinID " + PlayerHelper.Instance.skinID + ", indicatorID " + PlayerHelper.Instance.indicatorID);
        this.GetComponent<MeshRenderer>().material = skinDB.GetSkin(PlayerHelper.Instance.skinID);
        if (PlayerHelper.Instance.skinID != 9)
        {
            this.GetComponent<MeshRenderer>().material.color = PlayerHelper.Instance.skinColor;
        }

        powerupIndicator = Instantiate(indicatorDB.GetIndicator(PlayerHelper.Instance.indicatorID), this.transform.position, this.transform.rotation);
        powerupIndicatorRenderer = powerupIndicator.GetComponent<MeshRenderer>();
        powerupIndicatorBaseMaterial.color = PlayerHelper.Instance.indicatorColor;

        powerupIndicator.AddComponent<RotateAround>().speed = -0.5f;

        playerRb.mass += PlayerHelper.Instance.massIncrease;
        speed += PlayerHelper.Instance.speedIncrease;
        speed *= speedRatio;

        powerupCountdownText.gameObject.SetActive(false);
        joystick.gameObject.SetActive(true);
        castButton.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleMovement();
        HandlePowerup();
        CheckPlayerStandStill();
    }

    void HandleMovement()
    {
        Vector2 input = move.ReadValue<Vector2>();
        verticalInput = input.y;
        if (GameManager.Instance.isWaveStart && isGrounded())
        {
            playerRb.AddForce(focalPoint.transform.forward * speed * verticalInput);
        }

        //PowerupIndicator follow player
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);
    }

    void HandlePowerup()
    {
        DisplayTime(powerupRemaining);
        powerupRemaining -= Time.deltaTime;

        if (doPowerupPassive != null)
        {
            doPowerupPassive();
        }

        if (GameManager.Instance.isGameOver)
        {
            powerupCountdownText.gameObject.SetActive(false);
            powerupIndicatorRenderer.material = powerupIndicatorBaseMaterial;
        }
    }

    private void CastSkill(InputAction.CallbackContext context)
    {
        if (doPowerupCast != null)
        {
            doPowerupCast();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup") && GameManager.Instance.isWaveStart)
        {
            if (powerupController != null)
            {
                powerupController.enabled = false;
            }


            powerupCountdownText.gameObject.SetActive(true);

            currentPowerup = other.gameObject.GetComponent<Powerup>().powerupType;

            //Debug.Log(currentPowerup);
            powerupController = this.GetComponent(currentPowerup.ToString()) as PlayerPowerupSystem;
            powerupController.enabled = true;
            powerupTime = powerupController.powerupTime;
            powerupRemaining = powerupTime;

            Destroy(other.gameObject);
            powerupIndicator.GetComponent<MeshRenderer>().material = other.GetComponent<MeshRenderer>().material;
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
            powerupController.enabled = false;
        }

        powerupCountdownText.gameObject.SetActive(false);
        powerupIndicatorRenderer.material = powerupIndicatorBaseMaterial;

        currentPowerup = PowerupType.None;
        doPowerupCast = null;
        doPowerupPassive = null;
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

    void CheckPlayerStandStill()
    {
        if ((GameManager.Instance.isWaveStart) && (this.GetComponent<_1000tons>().enabled == false) && (playerRb.velocity.magnitude < 0.1f))
        {
            Debug.Log("Standing");
            if (!standStilltimerStarted)
            {
                standStilltimerStarted = true;
                standStilltimer = 0.0f;
            }
            else
            {
                standStilltimer += Time.deltaTime;
            }
            if (standStilltimer >= maxStandTime)
            {
                //this.gameObject.SetActive(false);
                standTooLong = true;
                standStilltimerStarted = false;
                standStilltimer = 0.0f;
            }
        }
        else
        {
            standTooLong = false;
            standStilltimerStarted = false;
        }
    }
}

