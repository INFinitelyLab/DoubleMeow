using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _surfSpeed;
    [SerializeField] private float _gravityScale;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _roadWidth;
    [SerializeField] private float _curveAroundHeight;
    [SerializeField] private float _curveAroundWidth;
    [SerializeField] private GroundChecker _grounder;

    private bool _isParachuteControl;
    private bool _isVehicleControl;
    private bool _isControlled = true;
    private bool _isCurveControl;
    private bool _isAIRControl;
    private bool _isNeedToEnableCurveControl;
    private bool _inPortal;

    private float _verticalIntensive;

    private Coroutine _jumpRountine;

    private RoadLine _line;


    private Vector3 _targetPosition;
    private Vector3 _turnPosition;
    private Vector3 _velocity;
    private Vector3 _previousControllerVelocity = Vector3.one;

    private CharacterController _controller;
    private Transform _transform;

    public Action<float> RepositeCamera;
    public Action Jumped;
    public Action<bool> Grounded;
    public Action<Direction, float> Redirected;

    public float walkSpeed => _walkSpeed;
    public RoadLine CurrentLine => _line;

    private bool grounded;
    private bool isFly;
    private bool isTurning;

    public Vector3 Position => Quaternion.Inverse(_transform.rotation) * _transform.position;
    public Vector3 TurnPosition => Quaternion.Inverse(_transform.rotation) * _turnPosition;
    public Vector3 TurnPositionWithoutRotation => _turnPosition;
    public Vector3 TargetPosition => _targetPosition;

    public CharacterController Controller => _controller;

    public Vector3 InterpolatePosition => Position;
    public Vector3 InterpolateTurnPosition => TurnPosition;

    public Vector3 LocalPosition => Position - TurnPosition;

    public bool IsTurning => isTurning;

    public float RotateProgress { get; private set; }
    public float Height { get; private set; }


    public void Jump()
    {
        if (_jumpRountine != null) StopCoroutine(_jumpRountine);

        _jumpRountine = StartCoroutine(HoldJump(1));
    }


    private bool TryJump()
    {
        if (Game.IsActive == false || _inPortal || _isControlled == false || isActiveAndEnabled == false) return false;

        if (_isVehicleControl == true || _isCurveControl)
            return false;

        if ( grounded && _velocity.y <= 0)
        {
            PerfectJumpDetector.OnJump();

            _velocity.y = _jumpForce;

            Jumped?.Invoke();

            return true;
        }
        
        return false;
    }

    public void JumpOnStart()
    {
        _velocity.y = _jumpForce / 1.5f;

        Jumped?.Invoke();
    }


    public void Land()
    {
        if (Game.IsActive == false || _inPortal || _isControlled == false || isActiveAndEnabled == false) return;

        if (_isVehicleControl == true || _isCurveControl)
            throw new Exception("Нельзя опускаться будучи в машине");

        if (_grounder.IsGrounded == false)
        {
            _velocity.y = -_jumpForce / 1.5f;
        }
    }


    public void Surf(Direction direction)
    {
        if (Game.IsActive == false || _inPortal || _isCurveControl || _isControlled == false || _isAIRControl || isActiveAndEnabled == false) return;

        if (direction.IsVertical())
            throw new Exception("Нельзя скользить по вертикали");

        if (_line.TrySurf(direction))
        {
            _targetPosition.x = _line.ToInt() * _roadWidth;

            Redirected?.Invoke( direction, _roadWidth );
        }
        else
        {
            _targetPosition.x = _line.ToInt() * _roadWidth + ((int)direction * 0.5f);

            Redirected?.Invoke(direction, 0.1f);
        }
    }


    public void Drag(float horizontal, float vertical)
    {
        if ((_isCurveControl == false && _isAIRControl == false ) || isActiveAndEnabled == false || _isControlled == false) return;

        if(Game.Mode.InxAxIxRxMode)
            _targetPosition.x = Mathf.Clamp(_targetPosition.x + horizontal, -1.3f, 1.3f);
        else
            _targetPosition.x = Mathf.Clamp(_targetPosition.x + horizontal, -1.3f, 1.3f);

        _verticalIntensive = Mathf.Clamp(_verticalIntensive + vertical, -1f, 1f);

        if (_isAIRControl == true) Player.Presenter.OnRedirection(Direction.Right, horizontal * 10);
    }


    private void OnDrawGizmos()
    {
        if(_transform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine( _transform.position, _transform.position + _transform.forward );

            Gizmos.color = Color.white;
            Gizmos.DrawLine(_transform.position, _transform.position + (_transform.rotation * _velocity.normalized));

            Gizmos.color = Color.green;
            Gizmos.DrawLine( _transform.position, _turnPosition );

            Gizmos.color = Color.blue;
            Gizmos.DrawLine( _transform.position + Vector3.up, _turnPosition + Vector3.up);
        }
    }


    //

    private float _fixedTimeOffset;
    private float _lastUpdateTime;

    public void GameUpdate()
    {
        if (Time.deltaTime - _fixedTimeOffset > 0)// && isTurning == false)
            Tick(Time.deltaTime - _fixedTimeOffset);
        
        _fixedTimeOffset = 0;
    
        _lastUpdateTime = Time.time;
    }

    private void FixedUpdate()
    {
        if( Time.time - _lastUpdateTime > Time.fixedDeltaTime)// && isTurning == false )
        {
            Tick(Time.fixedDeltaTime);

            _fixedTimeOffset += Time.fixedDeltaTime;
        }
    }


    //


    private void Tick(float deltaTime)
    {

        grounded = _controller.isGrounded;

        if (_isVehicleControl == false && _isCurveControl == false && _inPortal == false && isFly == false)
        {
            if( _isAIRControl == true )
            {
                if (_isParachuteControl == false)
                {
                    _velocity.y = 0;
                }
                else
                {
                    _velocity.y = -2f + _verticalIntensive;
                }
            }
            else if (_controller.isGrounded == false || _velocity.y > 0)
            {
                _velocity.y += Physics.gravity.y * _gravityScale * deltaTime;
            }
            else
            {
                _velocity.y = CameraSyncer.IsSyncNow? -10f : -0.01f;
            }
        }
        else
        {
            if (_isVehicleControl)
                _velocity.y = -0.5f - _transform.position.y;
            else
                _velocity.y = 0;
        }

        if (Game.IsActive)
        {
            if (_isCurveControl && isFly == false)
            {
                _velocity.y = (0.05f + _curveAroundHeight - Position.y + (-Mathf.Cos((TurnPosition.x - Position.x) * _curveAroundWidth) * _curveAroundHeight - 0.075f)) / deltaTime;
                Player.Presenter.OnRedraged( -30f * (TurnPosition.x - Position.x));
            }

            _velocity.x = ((TargetPosition.x + TurnPosition.x - Position.x) * _surfSpeed);

            if (_isControlled == true)
                _velocity.z = Mathf.Lerp(_velocity.z, _walkSpeed * Game.Difficulty, 10 * deltaTime);
            else
                _velocity.z = Mathf.Lerp(_velocity.z, 0, Game.Difficulty * 20 * deltaTime);
        }
        else
        {
            _velocity.x = 0;
            _velocity.z = Mathf.Lerp( _velocity.z , 0 , 6 * deltaTime);
        }

        _controller.Move( transform.rotation * _velocity * deltaTime);

        if (_isNeedToEnableCurveControl)
            if (Position.y < 1 - (Mathf.Cos(Position.x / 1.05f) * _curveAroundHeight))
            {
                _isNeedToEnableCurveControl = false;
                _isCurveControl = true;

                if (Game.Mode.InParachuteMode) Game.Mode.DisableParachuteMode();
            }

        if (grounded != _controller.isGrounded)
        {
            if (_isControlled == false && Game.IsActive)
            {
                Player.Detector.Bump(false);
            }

            Grounded?.Invoke(_controller.isGrounded);
            RepositeCamera?.Invoke(transform.position.y);
            Height = transform.position.y;

            if (Game.Mode.InParachuteMode) Game.Mode.DisableParachuteMode();
        }

        _previousControllerVelocity = _controller.velocity;

        if (_isCurveControl == false && _isAIRControl == false) _targetPosition.x = Mathf.Lerp( _targetPosition.x , _line.ToInt() * (Game.Mode.InVehicleMode? 1.1f : _roadWidth), 20 * deltaTime);
    }


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = transform;

        Shader.SetGlobalFloat("_MetroRotation", 0);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 between = Vector3.zero;

        Vector3 point = Quaternion.Inverse(_transform.rotation) * hit.point;

        between.x = Mathf.Abs(Position.x - point.x);
        between.y = Mathf.Abs(Position.y - point.y);
        between.z = Mathf.Abs(Position.z - point.z);

        if (hit.transform.TryGetComponent<Obstacle>(out var obctacle) || hit.gameObject.CompareTag("Vehicle"))
        {
            if (Player.Presenter.InHulkMode && obctacle is Dais == false)
            {
                Destroy(hit.gameObject);
            }
            else if (between.x > between.z * 1.5f && between.y > 0.1f)
            {
                Direction direction = Position.x > point.x ? Direction.Right : Direction.Left;

                Surf(direction);
            }
        }

        hit.transform.TryGetComponent<Building>(out var building);

        if
        (
            (Quaternion.Inverse(_transform.rotation) * _controller.velocity).z / Mathf.Min(Time.fixedDeltaTime, Time.deltaTime) < 0.01f &&
            Game.IsActive &&
            (Quaternion.Inverse(_transform.rotation) * _previousControllerVelocity).z / Mathf.Min(Time.fixedDeltaTime, Time.deltaTime) < 0.01f &&
            (Player.Presenter.InHulkMode == false || building != null) &&
            _velocity.z > _walkSpeed * Game.Difficulty / 1.5f
        )
        {
            Debug.Log("Velocity : " + _controller.velocity + " , RotatedVelocityZ : " + (Quaternion.Inverse(_transform.rotation) * _controller.velocity).z / Mathf.Min(Time.fixedDeltaTime, Time.deltaTime));

            Player.Detector.Bump( building != null );

            _velocity = new Vector3(0, _jumpForce / 3, Game.Mode.InVehicleMode ? -15f : -5f) / (_isControlled ? 2 : 1);
        }
    }


    public void Turn(Turner turner)
    {
        StartCoroutine( RotateAround( turner ));

    }


    private Quaternion _futureRotation;
    private Vector3 _futureTurnPosition;


    public IEnumerator RotateAround( Turner turner )
    {
        Builder.Instance.DestroyAllTrash(false);

        Direction direction = turner.Direction;

        Quaternion startRotation = _transform.rotation;
        Quaternion rotation = Quaternion.Euler(0, _transform.eulerAngles.y + (direction == Direction.Left ? -90 : 90), 0);

        Vector3 offset = turner.Offset.position;

        Vector3 startPoint = turner.StartPoint.position;
        Vector3 endPoint = turner.EndPoint.position;

        float distanceBtwPoints = Vector3.Distance(startPoint, endPoint);

        isTurning = true;

        RotateProgress = 0;

        Quaternion lastRotation = _transform.rotation;

        Player.Camera.EnableTurnMode(turner);

        while (RotateProgress < 90)
        {
            _transform.right = new Vector3(_transform.position.x - offset.x, 0, _transform.position.z - offset.z) * (direction == Direction.Left ? 1 : -1);

            _turnPosition = turner.Offset.position + startRotation * Vector3.Slerp(turner.Offset.InverseTransformPoint(startPoint), turner.Offset.InverseTransformPoint(endPoint), RotateProgress / 90);// + startRotation * turner.ToCenterDirection.normalized * Mathf.Pow(Mathf.Sin(RotateProgress / 90 * Mathf.PI) + 0.0000001f, 0.75f) * distanceBtwPoints / 4 * 0.85f;
            
            RotateProgress += (Mathf.Abs(_transform.eulerAngles.y.NormalizeRotation() - lastRotation.eulerAngles.y.NormalizeRotation()));

            if (RotateProgress >= 90)
            {
                _transform.rotation = rotation;
                _turnPosition = turner.EndPoint.position;
                RotateProgress = 90;
            }

            lastRotation = _transform.rotation;

            yield return null;
        }

        isTurning = false;

        RegenTrigger.Instance.MoveTo(endPoint);
        RegenTrigger.Instance.transform.rotation = rotation;

        Player.Camera.DisableTurnMode();

        Shader.SetGlobalFloat("_MetroRotation", rotation.eulerAngles.y);
    }


    private void OnEnable()
    {
        _targetPosition.x = Position.x - TurnPosition.x;

        _line = (RoadLine)Mathf.RoundToInt(_targetPosition.x);
    }

    public float GetDistanceTo(Vector3 position)
    {
        return ( Quaternion.Inverse( Builder.Instance.Rotation ) * (_transform.position - position) ).z;
    }


    public void ChangeFlyMode()
    {
        isFly = !isFly;

        _controller.Move( Vector3.up * ( 2.5f - _transform.position.y ));
    }


    public void OnRegenerate()
    {
        _isControlled = true;

        _velocity.z = 0;
        _previousControllerVelocity = Vector3.one;

        _controller.Move(Vector3.one * 0.01f);
    }


    public void EnableVehicleControl()
    {
        if (_isVehicleControl == true)
            return;

        _isVehicleControl = true;

        _controller.radius *= 2;

        _velocity.y = 0;
        if (isFly == false) _controller.Move( Vector3.up * (-0.5f - Position.y) );

        RepositeCamera?.Invoke(0);

        _walkSpeed *= 2f;
    }

    public void DisableVehicleControl()
    {
        if (_isVehicleControl == false)
            return;

        _isVehicleControl = false;

        _controller.radius /= 2;

        _line = (RoadLine)( TargetPosition.x / _roadWidth);
        _targetPosition.x = _line.ToInt() * _roadWidth;

        if (Drone.IsExist)
        if (Drone.Instance.IsEnabled == false)
        {
            _velocity.z = 30 * Game.Difficulty;
            _velocity.y = 6.7f;

            Player.Presenter.OnJump();
        }

        _walkSpeed /= 2f;
    }


    public void EnableCurveControl()
    {
        if (Game.InMiniGames)
            _isCurveControl = true;
        else
            _isNeedToEnableCurveControl = true;

        Grounded?.Invoke(true);
        RepositeCamera?.Invoke(0);

        Player.Presenter.EnableCurveMode();
    }

    public void DisableCurveControl(bool isFly = true, RoadLine line = RoadLine.Venus)
    {
        if (_isCurveControl == false)
            return;

        _isCurveControl = false;

        if (isFly)
        {
            if (Drone.Instance.IsEnabled == false)
            {
                _velocity.y = 6f - (Position.y * 1.5f);

                Player.Presenter.DisableCurveMode();
            }
        }
        else
        {
            _velocity.z /= Game.Difficulty;
            _velocity.x = 0;
            _targetPosition.x = _line.ToInt() * (Game.Mode.InVehicleMode ? 1.1f : _roadWidth);

            _isControlled = false;

            _controller.stepOffset = 0;
        
            _line = line;
        }

        Grounded?.Invoke(false);

        Player.Presenter.OnJump();
    }


    public void EnablePortalControl(Vector3 exitPortalPosition, float distanceToPortal)
    {
        _inPortal = true;

        _newLine = (RoadLine)(int)Mathf.Round(exitPortalPosition.x);

        Invoke("_ChangeLineTo", 0.2f / Game.Difficulty);

        _walkSpeed *= 3f;

        Player.Presenter.SetScale(false, 30f / distanceToPortal);
    }

    public void DisablePortalControl()
    {
        _inPortal = false;

        _walkSpeed /= 3f;

        _velocity.y = _jumpForce * 1.1f;

        _velocity.z = 20 * Game.Difficulty;

        Player.Presenter.SetScale(true, 5f * Game.Difficulty);
        Player.Presenter.OnJump();
    }


    public void EnablexAxIxRxControl()
    {
        _targetPosition.x = Position.x - TurnPosition.x;

        _isAIRControl = true;

        _velocity.z = 5;
    }

    public void DisablexAxIxRxControl()
    {
        _isAIRControl = false;
    }


    public void EnableParachuteControl()
    {
        _targetPosition.x = Mathf.Clamp(_targetPosition.x, -1.1f, 1.1f);

        _verticalIntensive = 0;

        _isAIRControl = true;
        _isParachuteControl = true;
    }

    public void DisableParachuteControl()
    {
        _line = (RoadLine)(Mathf.Clamp(Mathf.RoundToInt(TargetPosition.x / _roadWidth * 0.746f), -1, 1));

        _isAIRControl = false;
        _isParachuteControl = false;
    }


    public void EnableAccelerate() => _walkSpeed *= 2.5f;

    public void DisableAccelerate() => _walkSpeed /= 2.5f;


    public void SetMovementSpeed(float speed)
    {
        _velocity.z = speed;
    }


    private RoadLine _newLine;


    private System.Collections.IEnumerator HoldJump(float duration)
    {
        float time = 0;

        while( time < duration && TryJump() == false)
        {
            time += Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
    }


    private void _ChangeLineTo()
    {
        _line = _newLine;
    }
}