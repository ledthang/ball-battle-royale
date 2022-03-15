using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketBehaviour : MonoBehaviour
{
    private float speed = 15;
    private bool isFire;
    private float rocketStrength = 15;
    private float aliveTimer = 5;

    private void Update()
    {
        if (isFire)
        {
            Vector3 moveDirection = this.transform.forward;
            transform.position += moveDirection * speed * Time.deltaTime;
        }
    }

    public void Fire()
    {
        rocketStrength = (15 + GameManager.Instance.waveNumber) * 5;
        isFire = true;
        Destroy(this.gameObject, aliveTimer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Rigidbody targetRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 away = -collision.contacts[0].normal;
            targetRb.AddForce(away * rocketStrength, ForceMode.Impulse);
            Destroy(this.gameObject);
        }
    }

}
