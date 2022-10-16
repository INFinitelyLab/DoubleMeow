using UnityEngine;

public class Laser : MonoBehaviour
{
    private float _moveSpeed;

    private Transform _transform;
    private Vector3 _direction;


    public void Launch(Vector3 direction, float speed)
    {
        _direction = direction;

        _moveSpeed = speed;
    }


    private void Start()
    {
        _transform = transform;
    }


    private void FixedUpdate()
    {
        _transform.position += _direction * _moveSpeed * Time.fixedDeltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<IGroundeable>(out var ground))
        {
            if( other.gameObject.TryGetComponent<Player>(out var player) )
            {
                //Player.Detector.Bump();
            }

            Destroy(gameObject);
        }
    }
}