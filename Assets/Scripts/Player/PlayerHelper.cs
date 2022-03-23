using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerHelper : MonoBehaviour
{
    public static PlayerHelper Instance;

    private string _name;
    public string playerName
    {
        get
        {
            LoadName();
            return _name;
        }
        set
        {
            _name = value;
            SaveName();
        }
    }

    private int _skinID;
    public int skinID
    {
        get
        {
            LoadSkinID();
            return _skinID;
        }
        set
        {
            _skinID = value;
            SaveSkinID();
        }
    }

    public Color colorGrey = new Color(140, 140, 140);
    public Color colorYellow =  new Color(255, 194, 0);

    private Color _skinColor;
    public Color skinColor
    {
        get
        {
            LoadSkinColor();
            return _skinColor;
        }
        set
        {
            _skinColor = value;
            SaveSkinColor();
        }
    }


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this);

        //Debug.Log("GAME START");

        if (!PlayerPrefs.HasKey("playerName"))
        {
            playerName = "Player " + Random.Range(1000,9999).ToString();
        }
        else
        {
            LoadName();
        }

        if (!PlayerPrefs.HasKey("skinID"))
        {
            skinID = 0;
        }
        else
        {
            LoadSkinID();
        }

        if (!PlayerPrefs.HasKey("skinColor"))
        {
            skinColor = colorGrey;
        }
        else
        {
            LoadSkinColor();
        }

    }

    void SaveName()
    {
        PlayerPrefs.SetString("playerName", _name);
    }
    void LoadName()
    {
        _name = PlayerPrefs.GetString("playerName");
    }
    void SaveSkinID()
    {
        PlayerPrefs.SetInt("skinID", _skinID);
    }
    void LoadSkinID()
    {
        _skinID = PlayerPrefs.GetInt("skinID");
    }

    void SaveSkinColor()
    {
        string colorHexCode = ColorUtility.ToHtmlStringRGBA(_skinColor);
        PlayerPrefs.SetString("skinColor", colorHexCode);
    }
    void LoadSkinColor()
    {
        var colorHexCode = PlayerPrefs.GetString("skinColor");
        if (colorHexCode[0] != '#')
            colorHexCode = '#' + colorHexCode;

        Color color = colorGrey;

        if (ColorUtility.TryParseHtmlString(colorHexCode, out color))
        {
            //Debug.Log("Load skin color success");
            _skinColor = color;
        }
    }

}
