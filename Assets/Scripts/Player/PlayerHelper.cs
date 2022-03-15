using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerHelper : MonoBehaviour
{
    public static PlayerHelper Instance;

    //public GameObject player;

    private int _currentWave;
    public int currentWave
    {
        get
        {
            LoadWave();
            return _currentWave;
        }
        set
        {
            _currentWave = value;
            SaveWave();
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


    private int _indicatorID;
    public int indicatorID
    {
        get
        {
            LoadIndicatorID();
            return _indicatorID;
        }
        set
        {
            _indicatorID = value;
            SaveIndicatorID();
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


    private Color _indicatorColor;
    public Color indicatorColor
    {
        get
        {
            LoadIndicatorColor();
            return _indicatorColor;
        }
        set
        {
            _indicatorColor = value;
            SaveIndicatorColor();
        }
    }


    private int _massIncrease;
    public int massIncrease
    {
        get
        {
            LoadMass();
            return _massIncrease;
        }
        set
        {
            _massIncrease = value;
            SaveMass();
        }
    }


    private int _speedIncrease;
    public int speedIncrease
    {
        get
        {
            LoadSpeed();
            return _speedIncrease;
        }
        set
        {
            _speedIncrease = value;
            SaveSpeed();
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

        Debug.Log("GAME START");

        if (!PlayerPrefs.HasKey("currentWave"))
        {
            currentWave = 1;
        }
        else
        {
            LoadWave();
        }

        if (!PlayerPrefs.HasKey("massIncrease"))
        {
            massIncrease = 1;
        }
        else
        {
            //massIncrease = 1;
            LoadMass();
        }

        if (!PlayerPrefs.HasKey("speedIncrease"))
        {
            speedIncrease = 1;
        }
        else
        {
            //speedIncrease = 1;
            LoadSpeed();
        }

        if (!PlayerPrefs.HasKey("skinID"))
        {
            skinID = 0;
        }
        else
        {
            LoadSkinID();
        }

        if (!PlayerPrefs.HasKey("indicatorID"))
        {
            indicatorID = 0;
        }
        else
        {
            LoadIndicatorID();
        }

        if (!PlayerPrefs.HasKey("skinColor"))
        {
            skinColor = colorGrey;
        }
        else
        {
            LoadSkinColor();
        }

        if (!PlayerPrefs.HasKey("indicatorColor"))
        {
            indicatorColor = colorGrey;
        }
        else
        {
            LoadIndicatorColor();
        }
        Debug.Log("Start wave :" + currentWave);
    }

    void SaveWave()
    {
        PlayerPrefs.SetInt("currentWave", _currentWave);
    }
    void LoadWave()
    {
        _currentWave = PlayerPrefs.GetInt("currentWave");
    }

    void SaveMass()
    {
        PlayerPrefs.SetInt("massIncrease", _massIncrease);
    }
    void LoadMass()
    {
        _massIncrease = PlayerPrefs.GetInt("massIncrease");
    }

    void SaveSpeed()
    {
        PlayerPrefs.SetInt("speedIncrease", _speedIncrease);
    }
    void LoadSpeed()
    {
        _speedIncrease = PlayerPrefs.GetInt("speedIncrease");
    }

    void SaveSkinID()
    {
        PlayerPrefs.SetInt("skinID", _skinID);
    }
    void LoadSkinID()
    {
        _skinID = PlayerPrefs.GetInt("skinID");
    }

    void SaveIndicatorID()
    {
        PlayerPrefs.SetInt("indicatorID", _indicatorID);
    }
    void LoadIndicatorID()
    {
        _indicatorID = PlayerPrefs.GetInt("indicatorID");
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
            Debug.Log("Load skin color success");
            _skinColor = color;
        }
    }

    void SaveIndicatorColor()
    {
        PlayerPrefs.SetString("indicatorColor", ColorUtility.ToHtmlStringRGBA(_indicatorColor));
    }
    void LoadIndicatorColor()
    {
        var colorHexCode = PlayerPrefs.GetString("indicatorColor");
        if (colorHexCode[0] != '#')
            colorHexCode = '#' + colorHexCode;

        Color color = colorGrey;

        if (ColorUtility.TryParseHtmlString(colorHexCode, out color))
        {
            Debug.Log("Load indicator color success");
            _indicatorColor = color;
        }
    }


}
