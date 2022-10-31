using UnityEngine;

// ==========================================================================
// Это отладочный код для проверки работоспособности класса CurveObstacler
// ==========================================================================

// P.S удалить после замены кривых кривых на обычные кривые


public class CurveTester : MonoBehaviour
{
    [SerializeField] private Curve _curve;
    [SerializeField] private CurveObstacler _obstacler;

    private RoadLine line = RoadLine.Venus;


    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.G) )
        {
            SolarPanel[] panels = _curve.GetComponentsInChildren<SolarPanel>();

            foreach( SolarPanel panel in panels )
            {
                Destroy(panel.gameObject);
            }

            _obstacler.Generate(_curve, line);
        }
    }
}
