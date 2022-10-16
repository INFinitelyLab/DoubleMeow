using UnityEngine;

public class Obstacle : Placeable, IGroundeable
{
    
}


public abstract class Placeable : MonoBehaviour
{
    [SerializeField] private int _chance;

    public int chance => _chance;
}