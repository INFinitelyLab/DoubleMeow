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
    [SerializeField] private GroundChecker _grounder;

    private bool _isVehicleControl;
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
        if (Game.IsActive == false || _inPortal) return false;

        if (_isVehicleControl == true)
            return false;

        if ( _grounder.IsGrounded && _velocity.y <= 0)
        {
            _velocity.y = _jumpForce;

            Jumped?.Invoke();

            return true;
        }
        
        return false;
    }


    public void Land()
    {
        if (Game.IsActive == false || _inPortal) return;

        if (_isVehicleControl == true)
            throw new Exception("Нельзя опускаться будучи в машине");

        if (_grounder.IsGrounded == false)
        {
            _velocity.y = -_jumpForce;
        }
    }


    public void Surf(Direction direction)
    {
        if (Game.IsActive == false || _inPortal) return;

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


    private void FixedUpdate()
    {
        bool grounded = _controller.isGrounded;

        if (Game.Mode.InVehicleMode == false && _inPortal == false)
        {
            if (_controller.isGrounded == false || _velocity.y > 0)
            {
                _velocity.y += Physics.gravity.y * _gravityScale * Time.fixedDeltaTime;
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
            _velocity.x = (_targetPosition.x - _transform.position.x) * _surfSpeed * 1 * Time.fixedDeltaTime;
            _velocity.z = Mathf.Lerp(_velocity.z, _walkSpeed * Game.Difficulty * Time.fixedDeltaTime, 10 * Time.fixedDeltaTime);
        }
        else
        {
            _velocity.x = 0;
            _velocity.z = Mathf.Lerp( _velocity.z , 0 , 6 * Time.fixedDeltaTime );
        }

        _controller.Move(_velocity);

        if (_controller.velocity.z <= 0 && _transform.position.z > 3 && _velocity.z > 0)
        {
            Player.Detector.Bump();

            _velocity = new Vector3( 0, _jumpForce / 3, Game.Mode.InVehicleMode? -0.1f : -0.03f );
        }    

        if (grounded != _controller.isGrounded) Grounded?.Invoke( _controller.isGrounded );

        _targetPosition.x = Mathf.Lerp( _targetPosition.x , _line.ToInt() * (Game.Mode.InVehicleMode? 1.1f : _roadWidth), 20 * Time.fixedDeltaTime );
    }


    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _transform = transform;
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.TryGetComponent<Obstacle>(out var obctacle) == false && hit.gameObject.CompareTag("Vehicle") == false) return;

        Vector3 between = Vector3.zero;

        between.x = Mathf.Abs(_transform.position.x - hit.point.x);
        between.y = Mathf.Abs(_transform.position.y - hit.point.y);
        between.z = Mathf.Abs(_transform.position.z - hit.point.z);

        if ( between.x > between.z * 1.5f && between.x > between.z * 1.5f)
        {
            Direction direction = _transform.position.x > hit.point.x ? Direction.Right : Direction.Left;

            Surf(direction);
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
        _velocity.y = 0.1f;

        _walkSpeed /= 2f;
    }


    public void EnablePortalControl(Vector3 exitPortalPosition, float distanceToPortal)
    {
        _inPortal = true;

        _newLine = (RoadLine)(int)Mathf.Round(exitPortalPosition.x);

        Invoke("_ChangeLineTo", 0.3f / Game.Difficulty);

        _walkSpeed *= 3f;

        Player.Presenter.SetScale(0, 7f / distanceToPortal);
    }

    public void DisablePortalControl()
    {
        _inPortal = false;

        _walkSpeed /= 3f;

        _velocity.y = _jumpForce * 1.1f;

        _velocity.z = 20 * Time.fixedDeltaTime;

        Player.Presenter.SetScale(0.55f, 2.5f);
        Player.Presenter.OnJump();
    }

    private RoadLine _newLine;

    private void _ChangeLineTo()
    {
        _line = _newLine;
    }


    private System.Collections.IEnumerator HoldJump(float duration)
    {
        float time = 0;

        while( time < duration && TryJump() == false)
        {
            time += Time.fixedDeltaTime;

            Debug.Log("Hello");

            yield return new WaitForFixedUpdate();
        }
    }
}