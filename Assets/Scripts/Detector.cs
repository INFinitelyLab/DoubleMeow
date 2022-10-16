using UnityEngine;
using System;
using System.Collections.Generic;

public class Detector : MonoBehaviour
{
    public Action<Direction> Bumped;
    
    public void Bump() => Bumped?.Invoke(Direction.Up);
}