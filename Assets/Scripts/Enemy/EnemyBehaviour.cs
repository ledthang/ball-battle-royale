using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Rigidbody enemyRb;
    private GameObject player;

    [SerializeField] public float speed = 3;
    private float lowerBound = -10;
    private float sideBound = 20;
    private float topBound = 10;
    private float movementBound = 8.5f;
    //boss
    [SerializeField] public bool isBoss = false;
    [SerializeField] public float spawnInterval;
    private float nextSpawn;

    [SerializeField] public SpecialAbility specialAbility = SpecialAbility.None;
    bool isTouchedPlayer = false;
    public int miniEnemySpawnCount { get; set; }

    void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        //enemyRb.mass *= GameManager.Instance.waveNumber / SpawnManager.Instance.bossRound;
        if (isBoss)
        {
            enemyRb.mass *= 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
/*        else //if (player.gameObject.activeSelf == false)
        if (AdsManager.Instance.adsShow == true)
        {
            enemyRb.velocity = Vector3.zero;
            enemyRb.angularVelocity = Vector3.zero;
        }*/
        else
            switch (specialAbility)
            {
                case SpecialAbility.None:
                    HandleNormalEnemyMovement();
                    break;
                case SpecialAbility.Smart:
                    HandleSmartEnemyMovement();
                    break;
                case SpecialAbility.Dash:
                    HandleDashEnemyMovement();
                    break;
                case SpecialAbility.Runaway:
                    HandleRunawayEnemyMovement();
                    break;
            }

        DestroyOutOfBound();

        if (isBoss)
        {
            HandleBossWave();
        }
    }

    void HandleNormalEnemyMovement()
    {
        if (isGrounded() && GameManager.Instance.isWaveStart)
        {
            //enemy move
            Vector3 target;

            if (player.gameObject.transform.position.y < 0 || player == null)
            {
                target = Vector3.zero;
                //if game over => move to center of the island
            }
            else
            {
                target = player.transform.position;
            }
            Vector3 lookDirection = (target - enemyRb.position).normalized;

            enemyRb.AddForce(lookDirection * speed * enemyRb.mass);
        }
    }

    void HandleSmartEnemyMovement()
    {
        if (isGrounded() && GameManager.Instance.isWaveStart)
        {
            //enemy move
            Vector3 target;

            if (player.gameObject.transform.position.y < 0 || !IsMovementSafe() || player == null)
            {
                target = Vector3.zero;
                //if game over or movement not safe => move to center of the island
                isTouchedPlayer = false;
            }
            else
            {
                target = player.transform.position;
            }

            Vector3 lookDirection = (target - enemyRb.position).normalized;

            if (isTouchedPlayer && IsMovementSafe())
            {
                lookDirection = -lookDirection;
            }

            enemyRb.AddForce(lookDirection * speed * enemyRb.mass);
        }
    }

    void HandleDashEnemyMovement()
    {
        if (isGrounded() && GameManager.Instance.isWaveStart)
        {
            //enemy move
            Vector3 target;
            Vector3 lookDirection;

            if (player.gameObject.transform.position.y < 0 || !IsMovementSafe() || player == null)
            {
                target = Vector3.zero;
                //if game over or movement not safe => move to center of the island
                isTouchedPlayer = false;

                lookDirection = (target - enemyRb.position).normalized;

                enemyRb.AddForce(lookDirection * speed * enemyRb.mass);
            }
            else
            {
                RaycastHit hit;
                Physics.Linecast(transform.position, player.transform.position, out hit);
                if (hit.collider != null)
                {
                    // Debug.Log("is there any :" + hit.collider.name);

                    target = player.transform.position;

                    lookDirection = (target - enemyRb.position).normalized;

                    if (target == player.transform.position & target == hit.transform.position && !isTouchedPlayer)
                    {
                        Debug.Log(name + "DASHINGGGG");
                        enemyRb.AddForce(lookDirection * speed * enemyRb.mass * 5); //  *= add more dash force
                    }
                    else
                    {
                        if (IsMovementSafe())
                        {
                            enemyRb.AddForce(-lookDirection * speed * enemyRb.mass);
                        }
                    }
                }
            }
        }
    }

    void HandleRunawayEnemyMovement()
    {
        if (isGrounded() && GameManager.Instance.isWaveStart)
        {
            //enemy move
            Vector3 target;
            Vector3 lookDirection;
            if (player.gameObject.transform.position.y < 0 || !IsMovementSafe() || player == null)
            {
                target = Vector3.zero;
                lookDirection = (target - enemyRb.position).normalized;

                enemyRb.AddForce(lookDirection * speed * enemyRb.mass);
            }
            else
            {
                target = player.transform.position;
                lookDirection = target - enemyRb.position;

                if (lookDirection.magnitude < 5)
                {
                    lookDirection = (Quaternion.AngleAxis(90, Vector3.up) * lookDirection).normalized;
                    enemyRb.AddForce(lookDirection * speed * enemyRb.mass);
                }
            }
        }
    }

    bool IsMovementSafe()
    {
        float bound = (movementBound - enemyRb.velocity.magnitude);
        return (Mathf.Abs(this.transform.position.x) < bound) && (Mathf.Abs(this.transform.position.z) < bound);
    }

    void DestroyOutOfBound()
    {
        //destroy out of bound
        if ((transform.position.y < lowerBound) || (transform.position.x > sideBound) || (transform.position.z > sideBound) || (transform.position.y > topBound))
        {
            Destroy(gameObject);
            Debug.Log("Destroy " + name + "out of bound");
        }
    }

    void HandleBossWave()
    {
        //boss
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnInterval;
            SpawnManager.Instance.SpawnMiniEnemy(miniEnemySpawnCount);
        }
    }

    bool isGrounded()
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            AudioManager.Instance.PlayCollsionWithEnemySfx(this.transform.position);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            isTouchedPlayer = true;
            AudioManager.Instance.PlayCollsionWithEnemySfx(this.transform.position);
        }
        //collisionSound.Play();
    }
}
