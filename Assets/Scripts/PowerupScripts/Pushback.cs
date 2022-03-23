using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Push away enemy on collision
/// </summary>

public class Pushback : PlayerPowerupSystem
{
    float powerupStrength = 75;
    bool hasPowerup;
    //protected Color indicatorColor = new Color32(255, 238, 0, 255);
    protected override void OnEnable()
    {
        indicatorColor = new Color32(255, 238, 0, 255);
        base.OnEnable();
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

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ground") && hasPowerup)
        {
            AudioManager.Instance.PlayPowerupSfx(this.powerupType);
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = enemyRb.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            Debug.Log("Player collided with: " + collision.gameObject.name + " withpowerup set to Pushback");
        }
    }
}
