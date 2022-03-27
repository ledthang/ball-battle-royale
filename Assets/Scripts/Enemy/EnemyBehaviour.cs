using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum TypeOfEnemy
{
    Normal,
    Smart,
    Runaway
}

enum MovementState
{
    Stand,
    SelectDestination, //random choose to go to random pos or enemy
    SelectPosition,
    SelectEnemy,
    MoveToPosition,
    MoveToEnemy,
    Runaway
}

public class EnemyBehaviour : MonoBehaviour, IPlayer
{
    protected Rigidbody enemyRb;
    public GameObject nameText;
    Vector3 nameOffset = new Vector3(0, 1, 0);

    [SerializeField] public float speed = 3;
    float baseSpeed = 3;
    float baseMass = 10;
    private float lowerBound = -10;
    private float sideBound = 40;
    private float topBound = 10;
    private float movementBound = 17f;
    private float islandRadius = 20;

    public TypeOfEnemy typeOfEnemy;
    PlayerPowerupSystem powerupController;
    private Coroutine powerUpCountdown;
    float powerupTime = 7;
    float powerupRemaining;
    public PowerupType currentPowerup;

    List<IPlayer> touchedPlayer = new List<IPlayer>();

    public int point; //Killed enemy

    Vector3 currentTarget;
    GameObject currentEnemy;
    MovementState state;
    void OnEnable()
    {
        SpawnManager.Instance.enemyCount++;
    }
    void Start()
    {
        SetStartStat();
    }
    void SetStartStat()
    {
        enemyRb = this.GetComponent<Rigidbody>();
        nameText.GetComponent<TextMeshProUGUI>().text = this.name;

        speed = baseSpeed * enemyRb.mass;
        currentTarget = GenerateRandomPosition();
        state = MovementState.Stand;

        for (int i = 0; i < point; i++)
        {
            this.enemyRb.mass = baseMass + point * 2;
            this.speed = baseSpeed * this.enemyRb.mass;
            this.transform.localScale *= 1.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleCastSkill();

        //Handle Movement
        switch (typeOfEnemy)
        {
            case TypeOfEnemy.Normal:
                HandleNormalEnemyMovement();
                break;
            case TypeOfEnemy.Smart:
                HandleSmartEnemyMovement();
                break;
            case TypeOfEnemy.Runaway:
                HandleRunawayEnemyMovement();
                break;
        }

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

    #region MOVEMENT
    void MoveTo(Vector3 pos)
    {
        if (isGrounded())
        {
            Vector3 lookDirection = (pos - enemyRb.position).normalized;

            enemyRb.AddForce(lookDirection * speed * enemyRb.mass);
        }
    }
    void MoveAway(Vector3 pos)
    {
        if (isGrounded())
        {
            Vector3 lookDirection = (enemyRb.position - pos).normalized;

            enemyRb.AddForce(lookDirection * speed * enemyRb.mass);
        }
    }

    void RandomChangeState(float ratio)
    {
        if (Random.value > ratio)
        {
            state = (MovementState)Random.Range(0, 7);
        }
    }

    void HandleNormalEnemyMovement()
    {
        switch (state)
        {
            case MovementState.Stand:
                state = MovementState.SelectDestination;
                break;

            case MovementState.SelectDestination:
                if (Random.Range(0, 2) == 0)
                {
                    state = MovementState.SelectPosition;
                }
                else
                {
                    state = MovementState.SelectEnemy;
                }
                break;

            case MovementState.SelectPosition:
                currentTarget = GenerateRandomPosition();
                state = MovementState.MoveToPosition;
                break;

            case MovementState.MoveToPosition:
                if (this.transform.position == currentTarget)
                {
                    state = MovementState.Stand;
                }
                else
                {
                    MoveTo(currentTarget);
                    if (FindRandomTarget(5) != null)
                    {
                        state = MovementState.SelectEnemy;
                    }
                }
                break;

            case MovementState.SelectEnemy:
                currentEnemy = FindRandomTarget(5);
                if (currentEnemy != null)
                {
                    state = MovementState.MoveToEnemy;
                }
                else
                {
                    state = MovementState.SelectPosition; //no enemy found => go to random pos
                }
                break;

            case MovementState.MoveToEnemy:
                if (currentEnemy == null)
                {
                    state = MovementState.Stand;
                }
                else
                {
                    MoveTo(currentEnemy.transform.position);
                    RandomChangeState(0.9f);
                }
                break;

            default:
                state = MovementState.Stand;
                break;
        }
    }


    void HandleSmartEnemyMovement()
    {
        switch (state)
        {
            case MovementState.Stand:
                state = MovementState.SelectDestination;
                break;

            case MovementState.SelectDestination:
                if (Random.Range(0, 2) == 0)
                {
                    state = MovementState.SelectPosition;
                }
                else
                {
                    state = MovementState.SelectEnemy;
                }
                break;

            case MovementState.SelectPosition:
                currentTarget = GenerateRandomPosition();
                state = MovementState.MoveToPosition;
                break;

            case MovementState.MoveToPosition:
                if (!IsMovementSafe())
                {
                    //currentTarget = GenerateRandomPositionHalfRadius();
                    state = MovementState.SelectPosition;
                    MoveTo(currentTarget);
                }
                else if (this.transform.position == currentTarget)
                {
                    state = MovementState.Stand;
                }
                else
                {
                    MoveTo(currentTarget);
                    if (FindRandomTarget(5) != null)
                    {
                        state = MovementState.SelectEnemy;
                    }
                }
                break;

            case MovementState.SelectEnemy:
                currentEnemy = FindRandomTarget(5);
                if (currentEnemy != null)
                {
                    state = MovementState.MoveToEnemy;
                }
                else
                {
                    state = MovementState.SelectPosition; //no enemy found => go to random pos
                }
                break;

            case MovementState.MoveToEnemy:
                if (!IsMovementSafe())
                {
                    state = MovementState.MoveToPosition;
                }
                else if (currentEnemy == null)
                {
                    state = MovementState.Stand;
                }
                else
                {
                    MoveTo(currentEnemy.transform.position);
                    RandomChangeState(0.9f);
                }
                break;

            default:
                state = MovementState.Stand;
                break;
        }
    }

    void HandleRunawayEnemyMovement()
    {
        switch (state)
        {
            case MovementState.Stand:
                state = MovementState.SelectDestination;
                break;

            case MovementState.SelectDestination:
                if (Random.Range(0, 2) == 0)
                {
                    state = MovementState.SelectPosition;
                }
                else
                {
                    state = MovementState.SelectEnemy;
                }
                break;

            case MovementState.SelectPosition:
                currentTarget = GenerateRandomPosition();
                state = MovementState.MoveToPosition;
                break;

            case MovementState.MoveToPosition:
                if (!IsMovementSafe())
                {
                    //currentTarget = GenerateRandomPositionHalfRadius();
                    state = MovementState.SelectPosition;
                    MoveTo(currentTarget);

                }
                else if (this.transform.position == currentTarget)
                {
                    state = MovementState.Stand;
                }
                else
                {
                    MoveTo(currentTarget);
                    if (FindRandomTarget(5) != null)
                    {
                        state = MovementState.SelectEnemy;
                    }
                }
                break;

            case MovementState.SelectEnemy:
                currentEnemy = FindRandomTarget(5);
                if (currentEnemy != null)
                {
                    state = MovementState.Runaway;
                }
                else
                {
                    state = MovementState.SelectPosition; //no enemy found => go to random pos
                }
                break;

            case MovementState.Runaway:
                if (!IsMovementSafe())
                {
                    state = MovementState.MoveToPosition;
                }
                else if (currentEnemy == null)
                {
                    state = MovementState.Stand;
                }
                else
                {
                    MoveAway(currentEnemy.transform.position);
                    RandomChangeState(0.9f);
                }
                break;

            default:
                state = MovementState.Stand;
                break;
        }
    }
    #endregion

    #region POWERUP

    private void HandleCastSkill()
    {
        if (Random.value > 0.7f)
        {
            CastSkill();
        }
    }
    private void CastSkill()
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

        currentPowerup = PowerupType.None;
    }
    #endregion 
    private Vector3 GenerateRandomPositionHalfRadius()
    {
        float x, z;
        float radius = islandRadius / 2;
        x = Random.Range(-radius, radius);
        z = Random.Range(-radius, radius);

        return new Vector3(x, 0, z);
    }
    private Vector3 GenerateRandomPosition()
    {
        float x, z;
        x = Random.Range(-islandRadius, islandRadius);
        z = Random.Range(-islandRadius, islandRadius);

        return new Vector3(x, 0, z);
    }

    bool IsMovementSafe()
    {
        float bound = (movementBound - enemyRb.velocity.magnitude * transform.localScale.magnitude);
        return (Mathf.Abs(this.transform.position.x) < bound) && (Mathf.Abs(this.transform.position.z) < bound);
    }

    void DestroyOutOfBound()
    {
        //destroy out of bound
        if ((transform.position.y < lowerBound) || (transform.position.x > sideBound) || (transform.position.z > sideBound) || (transform.position.y > topBound))
        {
            foreach (var player in touchedPlayer)
            {
                MonoBehaviour mb = player as MonoBehaviour;
                if (mb != null)
                    //if (player != null)
                    player?.AddPoint();
            }

            Debug.Log("Destroy " + name + "out of bound");
            Destroy(nameText);
            Destroy(gameObject);
            SpawnManager.Instance.enemyCount--;
        }
    }

    public bool isGrounded()
    {
        RaycastHit hit;
        float distance = 2f;
        Vector3 dir = new Vector3(0, -2);

        if (Physics.Raycast(this.transform.position, dir, out hit, distance))
        {
            return true;
        }
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.PlayCollsionWithEnemySfx(this.transform.position);
            SetTouchedPlayer(collision.gameObject.GetComponent<IPlayer>());
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            RandomChangeState(0);
        }
        //collisionSound.Play();
    }

    GameObject FindRandomTarget(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(this.transform.position, radius);
        //Debug.Log(hits.Length);
        GameObject tmpObj = null;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].CompareTag("Powerup"))
            {
                return hits[i].transform.gameObject;
            }
            else if (hits[i].CompareTag("Player") && hits[i].transform.position.y > 0)
            {
                tmpObj = hits[i].transform.gameObject;
            }
        }

        return tmpObj;
    }

    public void AddPoint()
    {
        this.point++;
        if (this.transform.position.y > 0)
        {
            this.enemyRb.mass = baseMass + point * 2;
            this.speed = baseSpeed * this.enemyRb.mass;
            this.transform.localScale *= 1.1f;
        }
    }
    public int GetPoint()
    {
        return this.point;
    }

    public void SetTouchedPlayer(IPlayer player)
    {
        if (player != null)
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

    public float GetSpeed()
    {
        return this.speed;
    }
    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }
    public float GetMass()
    {
        return this.enemyRb.mass;
    }
    public void SetMass(float mass)
    {
        this.enemyRb.mass = mass;
    }
}
