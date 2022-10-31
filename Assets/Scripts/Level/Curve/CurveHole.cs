using UnityEngine;

public class CurveHole : MonoBehaviour
{
    [SerializeField] private RoadLine _line;

    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<Player>(out var player))
        {
            Player.Movement.DisableCurveControl(false, _line);
        }
    }
}
