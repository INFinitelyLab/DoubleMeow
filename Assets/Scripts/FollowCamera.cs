using UnityEngine;

public class FollowCamera : MonoBehaviour, ILowereable
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _moveSpeed;

    private Transform _transform;

    private Vector3 _position;
    private Vector3 _rotationOffset;


    private void Awake()
    {
        _transform = transform;

        _rotationOffset = _transform.localEulerAngles;

        _offset = _transform.position;

        _currentOffset = _offset;

        _idealOffset = _currentOffset;
    }


    private void LateUpdate()
    {
        if (_target == null)
            throw new System.Exception("Не назначен объект для преследования камерой");

        _position = _transform.position;

        _currentOffset = Vector3.Lerp(_currentOffset, _idealOffset, _moveSpeed / 3 * Time.deltaTime);

        _position.x = Mathf.Lerp(_position.x, (_target.position.x + _currentOffset.x) * (Game.Mode.InCurveMode? 0.75f : 1f), _moveSpeed * Time.deltaTime);
        _position.y = Mathf.Lerp(_position.y, (_lowers == 0? _target.position.y / 2 : 0) + _currentOffset.y, _moveSpeed * Time.deltaTime);

        _position.z = _target.position.z + _currentOffset.z;

        _transform.position = _position;

        _transform.localRotation = Quaternion.Euler( _rotationOffset.x, _rotationOffset.y , _rotationOffset.z + (Game.Mode.InCurveMode? _transform.position.x * 5 : 0) );
    }

    #region Lowereable

    private Vector3 _offset;
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