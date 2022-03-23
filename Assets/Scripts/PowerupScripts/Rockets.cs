using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shoot around player
/// </summary>
[System.Serializable]
public class Rockets : PlayerPowerupSystem, ICastable
{
    [SerializeField] GameObject rocketPrefab;
    private GameObject tmpRocket;
    bool isFire;
    Coroutine FireCoroutine;
    //protected Color indicatorColor = new Color32(255, 0, 0, 255);
    protected override void OnEnable()
    {
        indicatorColor = new Color32(255, 0, 0, 255);
        base.OnEnable();
        rocketPrefab = Resources.Load<GameObject>("Rocket");
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
    public override void Cast()
    {
        if (ammoLeft > 0 && !isFire)
        {
            isFire = true;
            ammoLeft--;
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
            var tmpPos = new Vector3(transform.position.x, 0.05f, transform.position.z);
            tmpRocket = Instantiate(rocketPrefab, tmpPos, Quaternion.Euler(tmpRot));
            tmpRocket.GetComponent<RocketBehaviour>().Fire(this.GetComponent<Rigidbody>());

            yield return new WaitForSeconds(0.05f);
        }
        isFire = false;
    }
}
