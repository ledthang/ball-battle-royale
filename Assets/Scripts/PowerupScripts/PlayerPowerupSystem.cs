using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public interface ICastable
{
    void Cast();
}
public class PlayerPowerupSystem : MonoBehaviour, ICastable
{
    [SerializeField] protected PowerupType powerupType;
    GameObject indicator;
    [SerializeField] protected GameObject indicatorPrefab;
    [SerializeField] protected Color indicatorColor = Color.red;

    public int powerupTime = 17;
    public int ammo = 1;
    // ammo = 0 : no cast skill
    // ammo = -1: inf cast 
    // ammo > 0 : cast n times, default = 3

    protected int ammoLeft;

    private void Awake()
    {
    }

    protected void Update()
    {
        indicator.transform.position = this.transform.position + new Vector3(0, -0.5f, 0);
        indicator.transform.localScale = new Vector3(3, 1, 3) * this.transform.localScale.x / 1.5f;
    }

    protected virtual void OnEnable()
    {
        AudioManager.Instance.PlayPowerupPickupSfx();

        if (ammo != 0)
        {
            ammoLeft = ammo;
            if (this.CompareTag("cPlayer"))
            {
                PlayerController.Instance.castButton.gameObject.SetActive(true);
            }
        }

        indicatorPrefab = Resources.Load<GameObject>("Indicator1");
        indicator = Instantiate(indicatorPrefab);
        indicator.GetComponent<MeshRenderer>().material.color = indicatorColor;
    }
    protected virtual void OnDisable()
    {
        if (this.CompareTag("cPlayer"))
        {
            Debug.Log(PlayerController.Instance.currentPowerup + " disabled");
            PlayerController.Instance.castButton.gameObject.SetActive(false);
        }
        Destroy(indicator);
    }

    public virtual void Cast()
    {
        if (this.CompareTag("cPlayer"))
        {
            if (ammoLeft == 0)
            {
                PlayerController.Instance.castButton.gameObject.SetActive(false);
            }
        }
    }
}
