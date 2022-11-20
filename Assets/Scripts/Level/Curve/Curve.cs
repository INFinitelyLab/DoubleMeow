using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Curve : Building
{
    [SerializeField] private Transform[] _positionsForSolarPanels;

    public Transform[] PositionsForSolarPanels => _positionsForSolarPanels;

    /*private Transform _player;
    private Transform _transform;

    private bool isActive;




    protected override void Update()
    {
        if (Vector3.Distance(_player.position, _transform.position) < 20 && isActive == false)
        {
            isActive = true;

            CurveObstacler.GenerateStatic(this);
        }
    }*/
}
