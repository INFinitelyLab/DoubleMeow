using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour, ILowereable
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _moveSmoothIntensive;
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Laser _laserPrefab;
    [SerializeField] private float _laserSpeed;

    private Transform _transform;

    private RoadLine _targetLine;
    private RoadLine _line;


    private void Start()
    {
        _transform = transform;

        _idealOffset = _offset;

        _currentOffset = _offset;

        StartCoroutine(ClampFire());

        Player.Movement.Redirected += Redirect;
    }


    private void OnDisable()
    {
        Player.Movement.Redirected -= Redirect;
    }


    private void Redirect(Direction direction, float force)
    {
        CancelInvoke("Reline");

        _targetLine.TrySurf(direction);

        Invoke("Reline", 0.3f);
    }


    private void Reline()
    {
        _line = _targetLine;
    }


    private void FixedUpdate()
    {
        _currentOffset = Vector3.Lerp(_currentOffset, _idealOffset, _moveSmoothIntensive * Time.fixedDeltaTime);

        _transform.position = Vector3.Lerp( _transform.position, _currentOffset + Vector3.forward *_player.position.z + (_lowers == 0? (Vector3.right * (int)_line * 0.65f) : Vector3.zero), _moveSmoothIntensive * Time.fixedDeltaTime );
    }


    private void Fire()
    {
        Vector3 direction = -(_transform.position - (_player.position + Vector3.forward * 1.5f)).normalized;

        direction.x = 0;

        Laser newLaser = Instantiate( _laserPrefab, _transform.position, Quaternion.LookRotation( direction ) );

        newLaser.Launch( direction, _laserSpeed );
    }


    private IEnumerator ClampFire()
    {
        while( gameObject != null )
        {
            if(_lowers == 0) Fire();

            yield return new WaitForSeconds(1.2f);
        }
    }



    #region Lowereable

    private Vector3 _currentOffset;
    private Vector3 _idealOffset;

    private byte _lowers;

    public void Low(float height)
    {
        _lowers++;

        _idealOffset.y = height;
    }

    public void Up()
    {
        _lowers--;

        if (_lowers == 0)
            _idealOffset.y = _offset.y;

    }

    #endregion
}