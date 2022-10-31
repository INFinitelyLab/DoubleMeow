using UnityEngine;
using System;

[RequireComponent(typeof(CharacterController))]
public class Movement : MonoBehaviour
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _surfSpeed;
    [SerializeField] private float _gravityScale;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _roadWidth;
    [SerializeField] private float _curveAroundHeight;
    [SerializeField] private GroundChecker _grounder;

    private bool _isVehicleControl;
    private bool _isControlled = true;
    private bool _isCurveControl;
    private bool _isNeedToEnableCurveControl;
    private bool _inPortal;

    private RoadLine _line;

    private Vector3 _targetPosition;
    private Vector3 _velocity;

    private CharacterController _controller;
    private Transform _transform;

    public Action Jumped;
    public Action<bool> Grounded;
    public Action<Direction, float> Redirected;

    public float walkSpeed => _walkSpeed;
    public RoadLine CurrentLine => _line;


    public void Jump()
    {
        StopAllCoroutines();

        StartCoroutine(HoldJump(1));
    }


    private bool TryJump()
    {
        if (Game.IsActive == false || _inPortal || _isControlled == false) return false;

        if (_isVehicleControl == true || _isCurveControl)
            return false;

        if ( _grounder.IsGrounded && _velocity.y <= 0)
        {
            PerfectJumpDetector.OnJump();

            _velocity.y = _transform.position.y + _jumpForce;

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


    public void Drag(float axis)
    {
        if (_isCurveControl == false || _isControlled == false) return;

        _targetPosition.x = axis * 1.5f;
    }


    private void Update()
    {
        bool grounded = _controller.isGrounded;

        if (_isVehicleControl == false && _isCurveControl == false && _inPortal == false)
        {
            if (_controller.isGrounded == false || _velocity.y > 0)
            {
                _velocity.y += Physics.gravity.y * _gravityScale * Time.deltaTime;
            }
            else
            {
                _velocity.y = -0.01f;
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
                _velocity.y = (1f - _transform.position.y + (-Mathf.Cos(_transform.position.x / 1.05f) * _curveAroundHeight)) / Time.deltaTime;
                Player.Presenter.OnRedraged( transform.position.y * 60f * (_transform.position.x > 0? 1 : -1));
            }

            _velocity.x = (_targetPosition.x - _transform.position.x) * _surfSpeed;
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

        _controller.Move( _velocity * Time.deltaTime);

        if (_isNeedToEnableCurveControl)
            if (_transform.position.y < 1 - (Mathf.Cos(_transform.position.x / 1.05f) * _curveAroundHeight))
            {
                _isNeedToEnableCurveControl = false;
                _isCurveControl = true;
            }

        if (_controller.velocity.z <= 0 && _transform.position.z > 3 && _velocity.z > 0 && Game.IsActive && _isControlled == true)
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
        }

        if (_isCurveControl == false) _targetPosition.x = Mathf.Lerp( _targetPosition.x , _line.ToInt() * (Game.Mode.InVehicleMode? 1.1f : _roadWidth), 20 * Time.deltaTime);
    }


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = transform;
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.TryGetComponent<Obstacle>(out var obctacle) || hit.gameObject.CompareTag("Vehicle"))
        {
            Vector3 between = Vector3.zero;

            between.x = Mathf.Abs(_transform.position.x - hit.point.x);
            between.y = Mathf.Abs(_transform.position.y - hit.point.y);
            between.z = Mathf.Abs(_transform.position.z - hit.point.z);

            if (between.x > between.z * 1.5f && between.x > between.z * 1.5f)
            {
                Direction direction = _transform.position.x > hit.point.x ? Direction.Right : Direction.Left;

                Surf(direction);
            }
        }
    }


    public void EnableVehicleControl()
    {
        if (_isVehicleControl == true)
            return;

        _isVehicleControl = true;

        _velocity.y = 0;
        _controller.Move( Vector3.up * (-0.25f - _transform.position.y) );


        _walkSpeed *= 2f;
    }

    public void DisableVehicleControl()
    {
        if (_isVehicleControl == false)
            return;

        _isVehicleControl = false;

        _line = (RoadLine)( _targetPosition.x / _roadWidth);
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

        Player.Presenter.EnableCurveMode();
    }

    public void DisableCurveControl(bool isFly = true, RoadLine line = RoadLine.Venus)
    {
        _isCurveControl = false;

        if (isFly)
        {
            _velocity.y = 6f - _transform.position.y;
            
            Player.Presenter.DisableCurveMode();
        }
        else
        {
            _velocity.z /= Game.Difficulty;
            _velocity.x = 0;

            _isControlled = false;
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

        Player.Presenter.SetScale(0.55f, 2.5f);
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