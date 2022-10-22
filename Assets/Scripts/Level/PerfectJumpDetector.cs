using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PerfectJumpDetector : Placeable
{
    private static Transform _currentDetector;

    private void OnTriggerEnter(Collider other)
    {
        if ( other.transform.TryGetComponent<Player>(out var player) )
        {
            _currentDetector = transform;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent<Player>(out var player))
        {
            if (_currentDetector == transform) _currentDetector = null;
        }
    }


    public static void OnJump()
    {
        if( _currentDetector != null )
        {
            bool isPerfectJump = Mathf.Abs(_currentDetector.position.z - Player.Movement.transform.position.z) < 0.4f;

            if (isPerfectJump)
                Stats.OnPerfectJump();
            else
                Stats.OnUnperfectJump();
        }
    }
}
