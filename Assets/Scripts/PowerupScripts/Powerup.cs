using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupType { None, Pushback, Rockets, Smash, Dash, _1000tons}
[System.Serializable]
public class Powerup : MonoBehaviour
{
    public PowerupType powerupType;    
}
