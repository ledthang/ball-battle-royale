using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// super heavy, stop instantly, nerf a little
/// </summary>
public class _1000tons : PlayerPowerupSystem // 1 ton only
{
    Rigidbody rb;
    float tmpMass;
    float tmpSpeed;
    float powerupStrength = 100;
    //protected Color indicatorColor = new Color32(0, 0, 0, 255);
    protected override void OnEnable()
    {
        indicatorColor = new Color32(0, 0, 0, 255);
        base.OnEnable();
        Debug.Log("1000tons enable");

        rb = this.GetComponent<Rigidbody>();

        //get current mass and speed
        tmpMass = rb.mass;
        tmpSpeed = PlayerController.Instance.speed;

        //set powerup attribute
        rb.mass = 1000f; //1 ton
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
    public override void Cast()
    {
        if (ammoLeft > 0)
        {
            AudioManager.Instance.PlayPowerupSfx(this.powerupType);

            ammoLeft--;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Debug.Log("STOPPPPP");

            Rigidbody[] enemies = FindObjectsOfType<Rigidbody>();
            for (int i = 0; i < enemies.Length; i++)
                if (enemies[i] != null & enemies[i] != rb)
                {
                    enemies[i].gameObject.GetComponent<IPlayer>()?.SetTouchedPlayer(this.gameObject.GetComponent<IPlayer>());
                    Vector3 lookDirection = (transform.position - enemies[i].transform.position).normalized;
                    enemies[i].AddForce(lookDirection * powerupStrength, ForceMode.Impulse);
                }
        }

        base.Cast();
    }
}
