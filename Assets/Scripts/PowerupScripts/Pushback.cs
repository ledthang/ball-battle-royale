using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Push away enemy on collision
/// </summary>

public class Pushback : PlayerPowerupSystem
{
    float powerupStrength = 15;
    bool hasPowerup;

    protected override void OnEnable()
    {
        base.OnEnable();
        powerupStrength = (15 + GameManager.Instance.waveNumber) * 5;
        hasPowerup = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        hasPowerup = false;
    }
    private void Start()
    {

    }

    public override void Passive()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasPowerup)
        {
            AudioManager.Instance.PlayPowerupSfx(this.powerupType);
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = enemyRb.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            Debug.Log("Player collided with: " + collision.gameObject.name + " withpowerup set to Pushback");
        }
    }
    public override void Cast()
    {
        base.Cast();
    }


}
