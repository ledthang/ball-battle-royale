using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEditor;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public Material currentSkin { get; private set; }
    public GameObject currentIndicator;

    [SerializeField] GameObject player;

    [SerializeField] TextMeshProUGUI waveText;

    [Header("Skin")]
    [SerializeField] PlayerSkinDB playerSkinDB;
    [SerializeField] Button nextSkinButton;
    [SerializeField] Button previousSkinButton;
    [SerializeField] public Button randomSkinColorButton;

    [Header("Indicator")]
    [SerializeField] PlayerPowerupIndicatorDB playerPowerupIndicatorDB;
    [SerializeField] Button nextIndicatorButton;
    [SerializeField] Button previousIndicatorButton;
    [SerializeField] public Button randomIndicatorColorButton;

    //mass, speed modify
    private int totalPoint
    {
        get
        {
            return PlayerHelper.Instance.currentWave - 1;
        }
    }
    private int pointLeft
    {
        get
        {
            return totalPoint - totalPointIncrease;
        }
    }

    private int totalPointIncrease
    {
        get
        {
            return PlayerHelper.Instance.massIncrease + PlayerHelper.Instance.speedIncrease;
        }
    }

    [Header("Player Attribute")]
    [SerializeField] Button increaseMassButton;
    [SerializeField] Button increaseSpeedButton;
    [SerializeField] TextMeshProUGUI massIncreaseText;
    [SerializeField] TextMeshProUGUI speedIncreaseText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
    }
    void Start()
    {
        //Debug.Log("Skin count: " + playerSkinDB.skinsCount);
        //Debug.Log("Indicator count: " + playerPowerupIndicatorDB.IndicatorCount);

        UpdateSkin(PlayerHelper.Instance.skinID);
        UpdateIndicator(PlayerHelper.Instance.indicatorID);
        PointLeftHandle();

        waveText.text = "" + PlayerHelper.Instance.currentWave;
    }

    public void NextSkin()
    {
        PlayerHelper.Instance.skinID++;
        if (PlayerHelper.Instance.skinID >= playerSkinDB.skinsCount)
        {
            PlayerHelper.Instance.skinID = playerSkinDB.skinsCount;
        }

        UpdateSkin(PlayerHelper.Instance.skinID);
    }

    public void PreviousSkin()
    {
        PlayerHelper.Instance.skinID--;
        if (PlayerHelper.Instance.skinID <= 0)
        {
            PlayerHelper.Instance.skinID = 0;
        }

        UpdateSkin(PlayerHelper.Instance.skinID);
    }

    private void UpdateSkin(int selectedOption)
    {
        //Debug.Log("Update current skin ID: " + selectedOption);

        player.GetComponent<MeshRenderer>().material = playerSkinDB.GetSkin(selectedOption);

        if (selectedOption != 9)
        {
            //Debug.Log("skin color: " + PlayerHelper.Instance.skinColor.ToString());
            player.GetComponent<MeshRenderer>().material.color = PlayerHelper.Instance.skinColor;
        }
        //PlayerHelper.Instance.skinID = selectedOption;

        previousSkinButton.interactable = (selectedOption > 0);
        nextSkinButton.interactable = (selectedOption < playerSkinDB.skinsCount
            && PlayerHelper.Instance.currentWave > playerSkinDB.GetSkinUnlockLevel(selectedOption + 1));
    }

    public void NextIndicator()
    {
        PlayerHelper.Instance.indicatorID++;
        if (PlayerHelper.Instance.indicatorID >= playerPowerupIndicatorDB.IndicatorCount)
        {
            PlayerHelper.Instance.indicatorID = playerPowerupIndicatorDB.IndicatorCount;
        }

        UpdateIndicator(PlayerHelper.Instance.indicatorID);
    }

    public void PreviousIndicator()
    {
        PlayerHelper.Instance.indicatorID--;
        if (PlayerHelper.Instance.indicatorID <= 0)
        {
            PlayerHelper.Instance.indicatorID = 0;
        }

        UpdateIndicator(PlayerHelper.Instance.indicatorID);
    }

    private void UpdateIndicator(int selectedOption)
    {
        Debug.Log("Update current indicator ID :" + selectedOption);

        var Indicator = playerPowerupIndicatorDB.GetIndicatorDB(selectedOption);

        currentIndicator.transform.GetChild(selectedOption).GetComponent<Renderer>().material.color = PlayerHelper.Instance.indicatorColor;

        //PlayerHelper.Instance.indicatorID = selectedOption;

        for (int ID = 0; ID <= playerPowerupIndicatorDB.IndicatorCount; ID++)
        {
            currentIndicator.transform.GetChild(ID).gameObject.SetActive(ID == Indicator.ID);
        }

        previousIndicatorButton.interactable = (selectedOption > 0);
        nextIndicatorButton.interactable = (selectedOption < playerPowerupIndicatorDB.IndicatorCount
            && PlayerHelper.Instance.currentWave > playerPowerupIndicatorDB.GetIndicatorUnlockLevel(selectedOption + 1));
        //Debug.Log(playerPowerupIndicatorDB.GetIndicatorUnlockLevel(selectedOption));
    }

    public void IncreaseMass()
    {
        PlayerHelper.Instance.massIncrease++;
        PointLeftHandle();
    }

    public void IncreaseSpeed()
    {
        PlayerHelper.Instance.speedIncrease++;
        PointLeftHandle();
    }

    private void PointLeftHandle()
    {
        massIncreaseText.text = "" + PlayerHelper.Instance.massIncrease;
        speedIncreaseText.text = "" + PlayerHelper.Instance.speedIncrease;

        increaseMassButton.interactable = (pointLeft > 0);
        increaseSpeedButton.interactable = (pointLeft > 0);
    }

    public void RefreshPoint()
    {
        PlayerHelper.Instance.massIncrease = 1;
        PlayerHelper.Instance.speedIncrease = 1;
        PointLeftHandle();
    }
    public void RandomSkinColor()
    {
        PlayerHelper.Instance.skinColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        Debug.Log("Random skin color successful");
        if (PlayerHelper.Instance.skinID != 9)
        {
            player.GetComponent<Renderer>().material.color = PlayerHelper.Instance.skinColor;
        }
    }

    public void RandomIndicatorColor()
    {
        Debug.Log("Random indicator color successful");
        PlayerHelper.Instance.indicatorColor
            = currentIndicator.transform.GetChild(PlayerHelper.Instance.indicatorID).GetComponent<Renderer>().material.color
            = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }

}
