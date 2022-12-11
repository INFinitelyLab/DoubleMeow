using UnityEngine;
using System.Collections;

public class Interpolation : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _syncTime = 20;

    private Transform _transform;

    private Vector3 _targetPosition;
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private Quaternion _targetRotation;
    private Quaternion _startRotation;
    private Quaternion _endRotation;

    private float _passedTime;
    private float _multiplier;

    public Vector3 Position => Quaternion.Inverse(_transform.rotation) * _transform.position;
    public Vector3 TurnPosition => Quaternion.Inverse(_transform.rotation) * Player.Movement.TurnPositionWithoutRotation;
    public Quaternion Rotation => _transform.rotation;


    private void OnEnable()
    {
        _transform = transform;

        StartCoroutine(IEFixedUpdate());
    }


    private void Update()
    {
        _passedTime += Time.deltaTime;

        _targetRotation = Quaternion.Lerp(_startRotation, _endRotation, _passedTime / Time.fixedDeltaTime);
        _targetPosition = Vector3.Lerp(_startPosition, _endPosition, _passedTime / Time.fixedDeltaTime);

        _transform.rotation = Quaternion.Lerp(_transform.rotation, _targetRotation , _syncTime * Time.deltaTime);
        _transform.position = Vector3.Lerp(_transform.position, _targetPosition , _syncTime * Time.deltaTime);
    }


    private IEnumerator IEFixedUpdate()
    {
        while(isActiveAndEnabled)
        {
            yield return new WaitForFixedUpdate();

            _startPosition = _targetPosition;
            _endPosition = _target.position;

            _startRotation = _targetRotation;
            _endRotation = _target.rotation;

            _passedTime = 0;
        }
    }
}
