using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    void AddPoint();
    int GetPoint();
    void SetTouchedPlayer(IPlayer playerTouched);
}
