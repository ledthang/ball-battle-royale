using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// jump and add explosion force around when grounded
/// </summary>
[System.Serializable]
public class Smash : PlayerPowerupSystem, ICastable
{
    float hangTime = 1;
    float smashSpeed = 3;
    float explosionForce = 150;
    float explosionRadius = 25;

    private bool smashing;
    private float floorY;

    Coroutine SmashCoroutine;
    //protected Color indicatorColor = new Color32(111, 110, 110, 255);
    protected override void OnEnable()
    {
        indicatorColor = new Color32(111, 110, 110, 255);
        base.OnEnable();
        smashing = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();


        if (SmashCoroutine != null)
        {
            StopCoroutine(SmashCoroutine);
        }

    }

    public override void Cast()
    {
        if (ammoLeft > 0 && !smashing)
        {
            Debug.Log("SMASHHHHHHHHHHHH");
            smashing = true;
            ammoLeft--;
            SmashCoroutine = StartCoroutine(DoSmash(this.gameObject));
        }

        base.Cast();
    }

    IEnumerator DoSmash(GameObject player)
    {
        var playerRb = player.GetComponent<Rigidbody>();
        var enemies = FindObjectsOfType<Rigidbody>();
        //Store the y position before taking off
        floorY = transform.position.y;
        //Calculate the amount of time we will go up
        float jumpTime = Time.time + hangTime;
        while (Time.time < jumpTime)
        {
            //move the player up while still keeping their x velocity.
            playerRb.velocity = new Vector2(playerRb.velocity.x, smashSpeed);
            yield return null;
        }
        //Now move the player down
        while (transform.position.y > floorY)
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, -smashSpeed * 2);
            yield return null;
        }

        if (transform.position.y <= floorY)
        {
            //play explosion sound
            AudioManager.Instance.PlayPowerupSfx(this.powerupType);

            //Cycle through all enemies.
            for (int i = 0; i < enemies.Length; i++)
            {
                //Apply an explosion force that originates from our position.
                if (enemies[i] != null && enemies[i] != playerRb)
                {
                    enemies[i].gameObject.GetComponent<IPlayer>()?.SetTouchedPlayer(this.gameObject.GetComponent<IPlayer>());
                    enemies[i].AddExplosionForce(explosionForce, transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
                }
            }

        }

        //Weare no longer smashing, so set the boolean to false
        smashing = false;
    }
}
