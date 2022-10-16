using UnityEngine;

public class FollowCamera : MonoBehaviour, ILowereable
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _moveSpeed;

    private Transform _transform;

    private Vector3 _position;


    private void Awake()
    {
        _transform = transform;

        _offset = _transform.position;

        _currentOffset = _offset;

        _idealOffset = _currentOffset;
    }


    private void Update()
    {
        if (_target == null)
            throw new System.Exception("Не назначен объект для преследования камерой");

        _position = _transform.position;

        _currentOffset = Vector3.Lerp(_currentOffset, _idealOffset, _moveSpeed / 3 * Time.deltaTime);

        _position.x = Mathf.Lerp(_position.x, _target.position.x + _currentOffset.x, _moveSpeed * Time.deltaTime);
        _position.y = Mathf.Lerp(_position.y, (_lowers == 0? _target.position.y : 0) + _currentOffset.y, _moveSpeed * Time.deltaTime);

        _position.z = _target.position.z + _currentOffset.z;

        _transform.position = _position;
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