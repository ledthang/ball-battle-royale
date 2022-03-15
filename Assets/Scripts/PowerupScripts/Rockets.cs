using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shoot around player
/// </summary>
[System.Serializable]
public class Rockets : PlayerPowerupSystem
{
    [SerializeField] GameObject rocketPrefab;
    private GameObject tmpRocket;
    bool isFire;
    Coroutine FireCoroutine;

    protected override void OnEnable()
    {
        base.OnEnable();

        isFire = false;
    }

    protected override void OnDisable()
    {
        base.OnDisable();


        if (FireCoroutine != null)
        {
            StopCoroutine(FireCoroutine);
        }

        foreach (var rocket in FindObjectsOfType<RocketBehaviour>())
        {
            Destroy(rocket.gameObject);
        }

    }

    public override void Passive()
    {
        base.Passive();
    }

    public override void Cast()
    {
        if (ammoLeft > 0 && !isFire)
        {
            isFire = true;
            ammoLeft--;
            ammoLeftText.text = ammoLeft + "/" + ammo;
            FireCoroutine = StartCoroutine(Fire());
            Debug.Log("FIREEEEEEEEEE");
        }

        base.Cast();
    }

    IEnumerator Fire()
    {
        var rot = this.transform.rotation.eulerAngles;
        for (float a = 0; a < 360; a += 20.0f)
        {
            AudioManager.Instance.PlayPowerupSfx(this.powerupType);

            var tmpRot = new Vector3(0, rot.y + a, 0);
            tmpRocket = Instantiate(rocketPrefab, transform.position, Quaternion.Euler(tmpRot));
            tmpRocket.GetComponent<RocketBehaviour>().Fire();

            yield return new WaitForSeconds(0.05f);
        }
        isFire = false;
    }
}
