using UnityEngine;

public class Curve : Building
{
    [SerializeField] private Material _materialForLowGraphics;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Transform[] _positionsForSolarPanels;
    [SerializeField] private Transform[] _positionsForStaticSolarPanels;

    public Transform[] PositionsForSolarPanels => _positionsForSolarPanels;
    public Transform[] PositionsForStaticSolarPanels => _positionsForStaticSolarPanels;


    private void Awake()
    {
        //if (Stats.TargetGraphics != GraphicPreset.High) _renderer.material = _materialForLowGraphics;
    }
}
