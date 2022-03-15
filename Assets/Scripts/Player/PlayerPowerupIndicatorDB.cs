using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerPowerupIndicatorDB : ScriptableObject
{
    public PlayerPowerupIndicator[] playerPowerupIndicators;

    public int IndicatorCount
    {
        get
        {
            return playerPowerupIndicators.Length - 1;
        }
    }

    public int GetIndicatorUnlockLevel(int ID)
    {
        return playerPowerupIndicators[ID].levelToUnlock;
    }

    public PlayerPowerupIndicator GetIndicatorDB(int ID)
    {
        return playerPowerupIndicators[ID];
    }

    public GameObject GetIndicator(int ID)
    {
        return playerPowerupIndicators[ID].powerupIndicator;
    }
}
