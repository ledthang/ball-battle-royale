using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    void AddPoint();
    int GetPoint();
    void SetTouchedPlayer(IPlayer playerTouched);

    float GetSpeed();
    void SetSpeed(float speed);
    float GetMass();
    void SetMass(float mass);
}
