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

    [SerializeField] GameObject player;

    [SerializeField] TextMeshProUGUI waveText;

    [Header("Skin")]
    [SerializeField] PlayerSkinDB playerSkinDB;
    [SerializeField] Button nextSkinButton;
    [SerializeField] Button previousSkinButton;
    [SerializeField] public Button randomSkinColorButton;

    [SerializeField] TMP_InputField playerName;

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
        UpdateSkin(PlayerHelper.Instance.skinID);
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
        nextSkinButton.interactable = (selectedOption < playerSkinDB.skinsCount);
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

    public void SubmitName(string arg0)
    {
        PlayerHelper.Instance.playerName = arg0;
    }
}
