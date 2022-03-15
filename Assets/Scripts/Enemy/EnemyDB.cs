using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyDB : ScriptableObject
{
    public Enemy[] enemies;

    public int enemiesCount
    {
        get
        {
            return enemies.Length;
        }
    }
}
