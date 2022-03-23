using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dash to random enemy
/// </summary>
public class Dash : PlayerPowerupSystem
{
    float dashStrength = 25;
    //protected Color indicatorColor = new Color32(35, 255, 0, 255);
    protected override void OnEnable()
    {
        indicatorColor = new Color32(35, 255, 0, 255);
        base.OnEnable();
        //dashStrength = (30 + GameManager.Instance.waveNumber) / 10;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Cast()
    {
        AudioManager.Instance.PlayPowerupSfx(this.powerupType);

        Rigidbody[] enemies = FindObjectsOfType<Rigidbody>();
        Vector3 target;
        if (enemies.Length > 0)
        {
            int randomIndex;
            int breakLoop = 0;
            do
            {
                randomIndex = Random.Range(0, enemies.Length);
                breakLoop++;
                if (breakLoop >= 10) break;
            } while (!enemies[randomIndex].CompareTag("Ground") && enemies[randomIndex].transform.position.y < -0.2f);
            if (breakLoop < 10)
            {
                Debug.Log("Dash target :" + enemies[randomIndex].name);
                target = enemies[randomIndex].gameObject.transform.position;
            }
            else target = Vector3.zero;
        }
        else
        {
            target = Vector3.zero;
        }
        Rigidbody rb = this.GetComponent<Rigidbody>();
        if (target != null)
        {
            Vector3 direction = (target - this.transform.position).normalized;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(direction * dashStrength, ForceMode.VelocityChange);
        }

        base.Cast();
    }
}
