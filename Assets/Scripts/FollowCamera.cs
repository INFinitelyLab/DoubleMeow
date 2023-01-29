using UnityEngine;

public class FollowCamera : MonoBehaviour, ILowereable
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Vector3 _offset;

    [Header("Tiler")]
    [SerializeField] private Vector3 _tilerPosition;
    [SerializeField] private Vector3 _tilerRotation;

    [Header("Metro")]
    [SerializeField] private Vector3 _metroPosition;
    [SerializeField] private Vector3 _metroRotation;
    [SerializeField] private AnimationCurve _metroCurve;

    [Header("Hulk Animation")]
    [SerializeField] private Vector3 _hulkPosition;
    [SerializeField] private Vector3 _hulkRotation;

    [Header("Turn")]
    [SerializeField] private AnimationCurve _turnCurve;
    [SerializeField] private AnimationCurve _turnCameraCurve;

    private Vector3 _currentTilerPosition;
    private Vector3 _currentTilerRotation;

    private Vector3 _currentMetroPosition;
    private Vector3 _currentMetroRotation;

    private Vector3 _currentHulkPosition;
    private Vector3 _currentHulkRotation;

    private Transform _transform;

    private Vector3 _position;
    private Vector3 _turnPosition;
    private Quaternion _rotation;
    private Quaternion _turnRotation;
    private Vector3 _localPosition;
    private Vector3 _rotationOffset;

    private float _height;
    private float _targetHeight;

    private float _metroCurveTime = 4;

    private bool _isTilerMode;
    private bool _isMetroMode;
    private bool _inHulkMode;
    private bool _inAIRMode;

    private bool _isTurnMode;
    private float _turnFarIntensity;

    public bool InTilerMode => _isTilerMode;
    public Vector3 Offset => _offset;

    public Vector3 byOutsidePosition;
    private float _byOutsideTime = 1;


    private void Awake()
    {
        _transform = transform;

        _localPosition = _transform.position;

        _rotationOffset = _transform.localEulerAngles;

        _currentOffset = _offset;

        _idealOffset = _currentOffset;
    }


    public void OnOutside()
    {
        _byOutsideTime = 0;
    }


    public void OnRegenerate(Vector3 point)
    {
        _currentTilerRotation = Vector3.zero;
        _currentTilerPosition = Vector3.zero;

        _currentOffset = _idealOffset;

        _rotation = Quaternion.Euler(_rotationOffset.x, _rotationOffset.y + _target.eulerAngles.y, _rotationOffset.z);

        _transform.rotation = _rotation;
        _transform.position = point + _target.rotation * _currentOffset;
    }


    private void OnEnable()
    {
        Player.RepositeCamera += SetHeight;

        _targetHeight = _target.position.y;
        _height = _targetHeight;

        _currentOffset.y = _transform.position.y - (_target.position.y);

        _localPosition = Quaternion.Inverse(_target.rotation) * (_transform.position - _turnPosition);
    }


    private void OnDisable()
    {
        Player.RepositeCamera-= SetHeight;
    }


    public void SetHeight(float height)
    {
        _targetHeight = height;
    }


    public void EnableTurnMode(Turner turner)
    {
        _isTurnMode = true;
    }


    public void DisableTurnMode()
    {
        _isTurnMode = false;

        _turnPosition.x = Player.Movement.TurnPositionWithoutRotation.x;
        _turnPosition.z = Player.Movement.TurnPositionWithoutRotation.z;
    }


    public void EnableTilerMode()
    {
        _isTilerMode = true;
    }


    public void DisableTilerMode()
    {
        _isTilerMode = false;
    }


    public void EnableMetroMode()
    {
        _isMetroMode = true;

        if (Game.InMiniGames == false) _metroCurveTime = 0;
    }


    public void DisableMetroMode(bool withAnimation)
    {
        _isMetroMode = false;

        if(withAnimation) _metroCurveTime = 0;
    }


    public void EnableHulkMode()
    {
        _inHulkMode = true;
    }

    public void DisableHulkMode()
    {
        _inHulkMode = false;
    }


    public void EnablexAxIxRxMode()
    {
        _inAIRMode = true;

        _height = 0;
        _currentOffset.y = _transform.position.y - (_target.position.y);

        _localPosition = Quaternion.Inverse(_target.rotation) * (_transform.position - _turnPosition);
    }

    public void DisablexAxIxRxMode()
    {
        _inAIRMode = false;

        _height = 0;

        _localPosition = Quaternion.Inverse(_target.rotation) * (_transform.position - _turnPosition);
    }

    public void LateUpdate()
    {
        if (_target == null)
            throw new System.Exception("Не назначен объект для преследования камерой");


        if (_isTurnMode)
        {
            _turnPosition.x = Player.Movement.TurnPositionWithoutRotation.x;
            _turnPosition.z = Player.Movement.TurnPositionWithoutRotation.z;
        }

        _height = Mathf.Lerp( _height, _targetHeight, 4 * Time.deltaTime );
        
        _currentTilerPosition = Vector3.Lerp(_currentTilerPosition, (_isTilerMode ? _tilerPosition : Vector3.zero), 2 * Time.deltaTime);
        _currentTilerRotation = Vector3.Lerp(_currentTilerRotation, (_isTilerMode ? _tilerRotation : Vector3.zero), 2 * Time.deltaTime);

        _currentMetroPosition = Vector3.Lerp(_currentMetroPosition, (_isMetroMode ? _metroPosition : Vector3.zero), 2 * Time.deltaTime);
        _currentMetroRotation = Vector3.Lerp(_currentMetroRotation, (_isMetroMode ? _metroRotation : Vector3.zero), 2 * Time.deltaTime);

        _currentOffset = Vector3.Lerp(_currentOffset, _idealOffset * ( _inHulkMode? 2 : 1 ), _moveSpeed / 3 * Time.deltaTime);

        _metroCurveTime = Mathf.MoveTowards( _metroCurveTime, 4, Time.deltaTime );
        _byOutsideTime = Mathf.MoveTowards( _byOutsideTime, 1, 2 * Time.deltaTime );

        //_turnFarIntensity = Mathf.Lerp(_turnFarIntensity, _turnCameraCurve.Evaluate( Player.Movement.RotateProgress / 90 ), 5 * Time.deltaTime);
        
            _localPosition.x = Mathf.Lerp(_localPosition.x, ((Player.Movement.LocalPosition.x) + _currentOffset.x) * (Game.Mode.InCurveMode ? 0.75f : 1f) , _moveSpeed * Time.deltaTime);
            _localPosition.y = Mathf.Lerp(_localPosition.y, _inAIRMode == false? ((_lowers == 0 ? (Mathf.Lerp(0, (_target.position.y), (_isMetroMode ? 1 - _metroCurveTime : _metroCurveTime)) - _height) / 2 : 0) + _currentMetroPosition.y + _currentHulkPosition.y + _currentOffset.y + _height + _currentTilerPosition.y) : (_target.position.y + _currentOffset.y) , _moveSpeed * Time.deltaTime);
            _localPosition.z = -_turnFarIntensity + _currentOffset.z + _currentTilerPosition.z + _currentMetroPosition.z + _currentHulkPosition.z + _metroCurve.Evaluate(_metroCurveTime * 2.5f) * (_isMetroMode ? 0.5f : 0.5f);

            if (Drone.Instance.IsEnabled == false && Game.Mode.InxAxIxRxMode == false && Game.Mode.InParachuteMode == false && Game.Mode.InCurveMode == false) _localPosition.y = Mathf.Max(_localPosition.y, _height + _currentOffset.y);

        Vector3 localPosition = (_currentOffset + Player.Movement.Position);

        localPosition.x = _localPosition.x + Player.Movement.TurnPosition.x;
        localPosition.y = _localPosition.y;
        localPosition.z = _localPosition.z + Player.Movement.Position.z;

        _transform.rotation = Quaternion.Euler(_rotationOffset.x + _currentTilerRotation.x + _currentMetroRotation.x + _currentHulkRotation.x, _rotationOffset.y + _target.eulerAngles.y, _rotationOffset.z + (Game.Mode.InCurveMode ? Player.Movement.LocalPosition.x * 5 : 0));

        _transform.position = Vector3.Lerp( byOutsidePosition , _target.rotation * localPosition , _byOutsideTime);
    }

    public void LatexAxIxRxUpdate()
    {
        _transform.localPosition = Vector3.Lerp( _transform.localPosition, _currentOffset, 5 * Time.deltaTime );
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