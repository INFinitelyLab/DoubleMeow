using UnityEngine;

public class PlayerTurner : MonoBehaviour
{
    private Direction _direction;

    private Vector3 _targetRotation = Vector3.zero;
    private Transform _transform;

    public Direction Direction => _direction;


    private void Awake()
    {
        _transform = transform;
    }


    public void TurnTo(Direction direction, Vector3 aroundPoint)
    {
        _transform.RotateAround(aroundPoint, Vector3.up, direction == Direction.Left ? -90 : 90);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) TurnTo( Direction.Left, Vector3.forward * Player.Movement.transform.position.z + Vector3.left );
    }
}