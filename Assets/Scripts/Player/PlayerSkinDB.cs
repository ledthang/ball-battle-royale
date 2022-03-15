using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSkinDB : ScriptableObject
{
    public PlayerSkin[] playerSkins;

    public int skinsCount
    {
        get
        {
            return playerSkins.Length - 1;
        }
    }

    public PlayerSkin GetSkinDB(int ID)
    {
        return playerSkins[ID];
    }

    public Material GetSkin(int ID)
    {
        return playerSkins[ID].playerSkin;
    }

    public int GetSkinUnlockLevel(int ID)
    {
        return playerSkins[ID].levelToUnlock;
    }
}
