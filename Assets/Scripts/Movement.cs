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
    [SerializeField] private float _rotateRadius = 15;
    [SerializeField] private float _curveAroundHeight;
    [SerializeField] private GroundChecker _grounder;

    private bool _isVehicleControl;
    private bool _isControlled = true;
    private bool _isCurveControl;
    private bool _isNeedToEnableCurveControl;
    private bool _inPortal;

    private Coroutine _jumpRountine;

    private RoadLine _line;

    private Quaternion _targetRotation;

    private Vector3 _targetPosition;
    private Vector3 _turnPosition;
    private Vector3 _velocity;

    private CharacterController _controller;
    private Transform _transform;

    public Action<float> RepositeCamera;
    public Action Jumped;
    public Action<bool> Grounded;
    public Action<Direction, float> Redirected;

    public float walkSpeed => _walkSpeed;
    public RoadLine CurrentLine => _line;

    private bool grounded;
    private bool isTurning;

    public Vector3 Position => Quaternion.Inverse(_transform.rotation) * _transform.localPosition;
    public Vector3 TurnPosition => Quaternion.Inverse(_transform.rotation) * _turnPosition;
    public Vector3 TurnPositionWithoutRotation => _turnPosition;
    public Vector3 TargetPosition => _targetPosition;

    public float RotateProgress { get; private set; }


    public void Jump()
    {
        if (_jumpRountine != null) StopCoroutine(_jumpRountine);

        _jumpRountine = StartCoroutine(HoldJump(1));
    }


    private bool TryJump()
    {
        if (Game.IsActive == false || _inPortal || _isControlled == false) return false;

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


    public void Land()
    {
        if (Game.IsActive == false || _inPortal || _isControlled == false) return;

        if (_isVehicleControl == true || _isCurveControl)
            throw new Exception("Нельзя опускаться будучи в машине");

        if (_grounder.IsGrounded == false)
        {
            _velocity.y = -_jumpForce / 1.5f;
        }
    }


    public void Surf(Direction direction)
    {
        if (Game.IsActive == false || _inPortal || _isCurveControl || _isControlled == false) return;

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


    public void Drag(float intensive)
    {
        if (_isCurveControl == false || _isControlled == false) return;

        _targetPosition.x = Mathf.Clamp(_targetPosition.x + intensive, -1.1f, 1.1f);
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
        }
    }


    private void Update()
    {
        grounded = _controller.isGrounded;

        if (_isVehicleControl == false && _isCurveControl == false && _inPortal == false)
        {
            if (_controller.isGrounded == false || _velocity.y > 0)
            {
                _velocity.y += Physics.gravity.y * _gravityScale * Time.deltaTime;
            }
            else
            {
                _velocity.y = CameraSyncer.IsSyncNow? -10f : -0.01f;
            }
        }
        else
        {
            _velocity.y = 0;
        }

        if (Game.IsActive)
        {
            if (_isCurveControl)
            {
                _velocity.y = (2.05f + _curveAroundHeight - Position.y + (-Mathf.Cos((TurnPosition.x - Position.x) * 1.1f) * _curveAroundHeight)) / Time.deltaTime;
                Player.Presenter.OnRedraged( (Position.y - 0.05f) * 60f * ((TurnPosition.x - Position.x) > 0? -1 : 1));
            }

            _velocity.x = ((TargetPosition.x + TurnPosition.x - Position.x) * _surfSpeed);

            if (_isControlled == true)
                _velocity.z = Mathf.Lerp(_velocity.z, _walkSpeed * Game.Difficulty, 10 * Time.deltaTime);
            else
                _velocity.z = Mathf.Lerp(_velocity.z, 0, Game.Difficulty * 20 * Time.deltaTime);
        }
        else
        {
            _velocity.x = 0;
            _velocity.z = Mathf.Lerp( _velocity.z , 0 , 6 * Time.deltaTime);
        }

        _controller.Move( transform.rotation * _velocity * Time.deltaTime);

        if (_isNeedToEnableCurveControl)
            if (Position.y < 1 - (Mathf.Cos(Position.x / 1.05f) * _curveAroundHeight))
            {
                _isNeedToEnableCurveControl = false;
                _isCurveControl = true;
            }

        if ((Quaternion.Inverse(_transform.rotation) * _controller.velocity).z <= 0.01f && _velocity.z > 0 && Game.IsActive && _isControlled == true)
        {
            Player.Detector.Bump();

            _velocity = new Vector3(0, _jumpForce / 3, Game.Mode.InVehicleMode ? -15f : -5f) / (_isControlled ? 2 : 1);
        }

        if (grounded != _controller.isGrounded)
        {
            if (_isControlled == false && Game.IsActive)
            {
                Player.Detector.Bump();
            }

            Grounded?.Invoke(_controller.isGrounded);
            RepositeCamera?.Invoke(transform.localPosition.y);
        }

        if (_isCurveControl == false) _targetPosition.x = Mathf.Lerp( TargetPosition.x , _line.ToInt() * (Game.Mode.InVehicleMode? 1.1f : _roadWidth), 20 * Time.deltaTime);
    }


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = transform;

        Shader.SetGlobalFloat("_MetroRotation", 0);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.TryGetComponent<Obstacle>(out var obctacle) || hit.gameObject.CompareTag("Vehicle"))
        {
            Vector3 between = Vector3.zero;
            Vector3 point = Quaternion.Inverse(_transform.rotation) * hit.point;

            between.x = Mathf.Abs(Position.x - point.x);
            between.y = Mathf.Abs(Position.y - point.y);
            between.z = Mathf.Abs(Position.z - point.z);

            if (between.x > between.z * 1.5f)
            {
                Direction direction = Position.x > point.x ? Direction.Right : Direction.Left;

                Surf(direction);
            }
        }
    }


    public void Turn(Turner turner)
    {
        StartCoroutine( RotateAround( turner ));

    }

    public IEnumerator RotateAround( Turner turner )
    {
        Direction direction = turner.Direction;

        Vector3 around = turner.transform.position;

        Vector3 offset = turner.Offset.position;
        Vector3 toCenterDirection = (turner.transform.position - turner.Offset.position) / 2;

        Quaternion rotation = Quaternion.Euler(0, _transform.eulerAngles.y + (direction == Direction.Left ? -90 : 90), 0);

        RegenTrigger.Instance.MoveTo( turner.EndPoint.position );
        RegenTrigger.Instance.transform.rotation = rotation;
        
        isTurning = true;

        RotateProgress = 0;

        _transform.right = new Vector3(_transform.position.x - offset.x, 0, _transform.position.z - offset.z) * (direction == Direction.Left ? 1 : -1);

        Player.Camera.EnableTurnMode(turner);

        Quaternion lastRotation = _transform.rotation;

        while (RotateProgress < 90 )
        {
            _turnPosition = Vector3.Lerp( turner.StartPoint.position, turner.EndPoint.position, RotateProgress / 90 ) + toCenterDirection * Mathf.Sin( RotateProgress / 90 * Mathf.PI ) / 2;
            
            _transform.right = new Vector3( _transform.position.x - offset.x, 0, _transform.position.z - offset.z) * (direction == Direction.Left ? 1 : -1);

            RotateProgress += (Mathf.Abs(_transform.eulerAngles.y.NormalizeRotation() - lastRotation.eulerAngles.y.NormalizeRotation()));

            if (RotateProgress >= 90) _transform.rotation = rotation;

            lastRotation = _transform.rotation;

            yield return null;
        }

        _turnPosition = around;

        Player.Camera.DisableTurnMode();

        Shader.SetGlobalFloat("_MetroRotation", rotation.eulerAngles.y);

        isTurning = false;
    }


    public void EnableVehicleControl()
    {
        if (_isVehicleControl == true)
            return;

        _isVehicleControl = true;

        _velocity.y = 0;
        _controller.Move( Vector3.up * (-0.5f - Position.y) );

        RepositeCamera?.Invoke(0);

        _walkSpeed *= 2f;
    }

    public void DisableVehicleControl()
    {
        if (_isVehicleControl == false)
            return;

        _isVehicleControl = false;

        _line = (RoadLine)( TargetPosition.x / _roadWidth);
        _targetPosition.x = _line.ToInt() * _roadWidth;
        _velocity.z = 30 * Game.Difficulty;
        _velocity.y = 6.7f;

        Player.Presenter.OnJump();

        _walkSpeed /= 2f;
    }


    public void EnableCurveControl()
    {
        _isNeedToEnableCurveControl = true;

        Grounded?.Invoke(true);
        RepositeCamera?.Invoke(0);

        Player.Presenter.EnableCurveMode();
    }

    public void DisableCurveControl(bool isFly = true, RoadLine line = RoadLine.Venus)
    {
        _isCurveControl = false;

        if (isFly)
        {
            _velocity.y = 6f - (Position.y * 1.5f);
            
            Player.Presenter.DisableCurveMode();
        }
        else
        {
            _velocity.z /= Game.Difficulty;
            _velocity.x = 0;
            _targetPosition.x = _line.ToInt() * (Game.Mode.InVehicleMode ? 1.1f : _roadWidth);

            _isControlled = false;

            _controller.stepOffset = 0;
        }

        _line = line;

        Grounded?.Invoke(false);

        Player.Presenter.OnJump();
    }


    public void EnablePortalControl(Vector3 exitPortalPosition, float distanceToPortal)
    {
        _inPortal = true;

        _newLine = (RoadLine)(int)Mathf.Round(exitPortalPosition.x);

        Invoke("_ChangeLineTo", 0.2f / Game.Difficulty);

        _walkSpeed *= 3f;

        Player.Presenter.SetScale(0.04f, 7f / distanceToPortal);
    }

    public void DisablePortalControl()
    {
        _inPortal = false;

        _walkSpeed /= 3f;

        _velocity.y = _jumpForce * 1.1f;

        _velocity.z = 20 * Game.Difficulty;

        Player.Presenter.SetScale(0.7f, 2.5f);
        Player.Presenter.OnJump();
    }


    public void EnableAccelerate() => _walkSpeed *= 2.5f;

    public void DisableAccelerate() => _walkSpeed /= 2.5f;


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