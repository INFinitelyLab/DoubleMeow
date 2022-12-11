using UnityEngine;
using System;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
    public Action<Direction, bool> Bumped;
    
    public void Bump(bool isNeedToDisactiveCamera) => Bumped?.Invoke(Direction.Up, isNeedToDisactiveCamera);
}