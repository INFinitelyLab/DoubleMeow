using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CameraSyncer : MonoBehaviour
{
    [SerializeField] private float _slope;

    private bool _isNeedToSync;
    private Transform _player;

    public static bool IsSyncNow { get; private set; }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<Player>(out var player))
        {
            _isNeedToSync = true;

            IsSyncNow = true;

            _player = Player.Movement.transform;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent<Player>(out var player))
        {
            _isNeedToSync = false;

            IsSyncNow = false;
        }
    }


    private void Update()
    {
        if (_isNeedToSync)
        {
            Player.Camera.SetHeight(_player.position.y);

            Player.Presenter.OnSlope( _slope );
        }
    }


    public static void Reset()
    {
        IsSyncNow = false;
    }
}
