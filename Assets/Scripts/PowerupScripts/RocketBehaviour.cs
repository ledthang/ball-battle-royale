using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    private float speed = 15;
    private bool isFire;
    private float rocketStrength = 125;
    private float aliveTimer = 5;
    private Rigidbody firedPlayer;
    private void Update()
    {
        if (isFire)
        {
            Vector3 moveDirection = this.transform.forward;
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    public void Fire(Rigidbody player)
    {
        firedPlayer = player;
        isFire = true;
        Destroy(this.gameObject, aliveTimer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody targetRb = collision.gameObject.GetComponent<Rigidbody>();
        if (targetRb != firedPlayer)
        {
            targetRb.gameObject.GetComponent<IPlayer>()?.SetTouchedPlayer(firedPlayer.gameObject.GetComponent<IPlayer>());
            Vector3 away = -collision.contacts[0].normal;
            targetRb.AddForce(away * rocketStrength, ForceMode.Impulse);
            Destroy(this.gameObject);
        }
    }
}
