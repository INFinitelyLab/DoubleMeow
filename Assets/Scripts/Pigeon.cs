using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Pigeon : MonoBehaviour
{
    public UnityEvent OnFlyed;

    private Transform _transform;
    private Vector3 _targetPosition;
    private bool isFlyed;
    private const float FlySpeed = 3;
    private float _currentSpeed = 1;


    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<Player>(out var player) && isFlyed == false)
        {
            FlyAway();
        }
    }


    public void FlyAway()
    {
        OnFlyed?.Invoke();

        isFlyed = true;

        _transform = transform;

        _targetPosition = _transform.position + _transform.forward * Random.Range(5, 8) + Vector3.up * Random.Range(5, 8);
    }


    private void FixedUpdate()
    {
        if (isFlyed == false) return;

        _currentSpeed = Mathf.MoveTowards(_currentSpeed, FlySpeed, 10 * Time.fixedDeltaTime);
        _transform.position = Vector3.MoveTowards( _transform.position, _targetPosition, _currentSpeed * Time.fixedDeltaTime );
    }
}