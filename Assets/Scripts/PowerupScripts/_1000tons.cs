using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// super heavy, stop instantly
/// </summary>
public class _1000tons : PlayerPowerupSystem
{
    Rigidbody rb;
    float tmpMass;
    float tmpSpeed;
    float powerupStrength = 10;
    protected override void OnEnable()
    {
        base.OnEnable();
        Debug.Log("1000tons enable");

        rb = this.GetComponent<Rigidbody>();

        //get current mass and speed
        tmpMass = rb.mass;
        tmpSpeed = PlayerController.Instance.speed;
        
        //set powerup attribute
        rb.mass = 1000000f; //1000 tons
        PlayerController.Instance.speed = rb.mass;

        //stop instantly
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //powerupStrength = GameManager.Instance.waveNumber * 10;

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        //return player attribute
        rb.mass = tmpMass;
        PlayerController.Instance.speed = tmpSpeed;
    }

    public override void Passive()
    {
        base.Passive();
    }

    public override void Cast()
    {
        if (ammoLeft > 0)
        {
            AudioManager.Instance.PlayPowerupSfx(this.powerupType);

            ammoLeft--;
            ammoLeftText.text = ammoLeft + "/" + ammo;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Debug.Log("STOPPPPP");

            EnemyBehaviour[] enemies = FindObjectsOfType<EnemyBehaviour>();
            for (int i = 0; i<enemies.Length; i++)
            {
                Vector3 lookDirection = (transform.position - enemies[i].transform.position).normalized;
                enemies[i].GetComponent<Rigidbody>().AddForce(lookDirection * powerupStrength, ForceMode.VelocityChange);
            }
        }

        base.Cast();
    }
}
