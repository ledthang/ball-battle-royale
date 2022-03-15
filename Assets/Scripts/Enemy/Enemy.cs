using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SpecialAbility
{
    None,
    Smart,
    Dash,
    Runaway
}

[System.Serializable]
public class Enemy
{
    public int ID;
    //physics
    public float massRatio = 1;
    public float speedRatio = 3;
    public float scaleRatio = 1.5f;
    public Material material;
    public Color color;
    //gameplay
    public int levelToUnlock;
    public bool isBoss = false;
    public float spawnInterval = 0;
    public SpecialAbility specialAbility = SpecialAbility.None;

    public string description;
}
