using UnityEngine;
using System.Collections;


public class StartPlatform : SingleBehaviour<StartPlatform>
{
    [SerializeField] private Transform _hex;
    [SerializeField] private Transform _hexEndPoint;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private AnimationCurve _animationCurve;
    [SerializeField] private AnimationCurve _cameraCurve;


    private void Start()
    {
        Player.Camera.enabled = false;
        Player.Movement.enabled = false;
        Player.Presenter.Animator.enabled = false;
        Player.Movement.transform.parent = _hex;
        Player.Camera.transform.parent = _hex;

        StartCoroutine( CameraFluid() );
    }

    public void OnGameStart()
    {
        StartCoroutine( Animate() );
    }


    private IEnumerator CameraFluid()
    {
        Transform camera = Player.Camera.transform;
        Vector3 position = camera.position;
        Vector3 addPosition = new Vector3();

        float time = 0;

        while( Game.IsActive == false )
        {
            time += Time.deltaTime / 3;

            addPosition.x = Mathf.Sin( time * Mathf.PI ) * 0.02f;
            addPosition.y = Mathf.Cos( time * Mathf.PI ) * 0.02f;
            camera.position = position + addPosition;

            yield return null;
        }
    }


    private IEnumerator Animate()
    {
        const float veryImpotantValue = 0.9f;

        Player.Camera.enabled = false;
        Player.Movement.enabled = false;
        Player.Presenter.Animator.enabled = false;
        Player.Movement.transform.parent = _hex;
        Player.Camera.transform.parent = _hex;

        Vector3 cameraStartPosition = Player.Camera.transform.localPosition;
        Vector3 cameraEndPosition = Player.Camera.Offset;
        Quaternion playerStartRotation = Player.Movement.transform.rotation;
        Quaternion playerEndRotation = Quaternion.identity;
        Vector3 hexStartPosition = _hex.position;

        float time = 0;

        while ( time < 1 )
        {
            time += Time.deltaTime * _moveSpeed;

            Player.Movement.transform.rotation = Quaternion.Lerp( playerStartRotation, playerEndRotation, _animationCurve.Evaluate(time) );

            _hex.position = Vector3.Lerp( hexStartPosition , _hexEndPoint.position, _animationCurve.Evaluate( time ) );

            Player.Camera.transform.localPosition = Vector3.Lerp( cameraStartPosition, cameraEndPosition, _animationCurve.Evaluate(time * veryImpotantValue)) + Vector3.back * _cameraCurve.Evaluate(_animationCurve.Evaluate(time * veryImpotantValue));

            yield return null;
        }

        _hex.position = _hexEndPoint.position;

        Player.Camera.enabled = true;
        Player.Movement.enabled = true;
        Player.Presenter.Animator.enabled = true;
        Player.Movement.transform.parent = null;
        Player.Camera.transform.parent = null;

        Player.Camera.OnOutside();
        Player.Movement.JumpOnStart();

        while ( time * veryImpotantValue < 1 )
        {
            time += Time.deltaTime * _moveSpeed;

            Player.Camera.byOutsidePosition = _hex.position + Vector3.Lerp(cameraStartPosition, cameraEndPosition, _animationCurve.Evaluate(time * veryImpotantValue)) + Vector3.back * _cameraCurve.Evaluate(_animationCurve.Evaluate(time * veryImpotantValue));

            yield return null;
        }
    }


    private void OnEnable()
    {
        Game.Started += OnGameStart;
    }


    private void OnDisable()
    {
        Game.Started -= OnGameStart;
    }
}
