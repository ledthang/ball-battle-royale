using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPowerupSystem : MonoBehaviour
{
    [SerializeField] protected PowerupType powerupType;

    public int powerupTime = 7;
    public int ammo = 3;
    // ammo = 0 : no cast skill
    // ammo = -1: inf cast 
    // ammo > 0 : cast n times, default = 3

    protected TextMeshProUGUI ammoLeftText;

    protected int ammoLeft;

    private void Awake()
    {
        ammoLeftText = GameObject.Find("Canvas").transform.Find("Ammo Left").GetComponent<TextMeshProUGUI>();
    }

    protected void Update()
    {
        if (GameManager.Instance.isGameOver)
        {
            gameObject.SetActive(false);
        }
    }

    protected virtual void OnEnable()
    {
        AudioManager.Instance.PlayPowerupPickupSfx();

        Debug.Log(PlayerController.Instance.currentPowerup + " enabled");
        PlayerController.doPowerupPassive += Passive;
        PlayerController.doPowerupCast += Cast;


        if (ammo != 0)
        {
            ammoLeft = ammo;

            PlayerController.Instance.castButton.gameObject.SetActive(true);

            if (ammo > 0)
            {
                ammoLeftText.gameObject.SetActive(true);
                ammoLeftText.text = ammoLeft + "/" + ammo;
            }
        }
    }
    protected virtual void OnDisable()
    {
        Debug.Log(PlayerController.Instance.currentPowerup + " disabled");
        PlayerController.doPowerupPassive -= Passive;
        PlayerController.doPowerupCast -= Cast;

        PlayerController.Instance.castButton.gameObject.SetActive(false);

        ammoLeftText.gameObject.SetActive(false);
    }

    public virtual void Passive()
    {

    }

    public virtual void Cast()
    {
        if (ammoLeft == 0)
        {
            PlayerController.Instance.castButton.gameObject.SetActive(false);
        }
    }

}
