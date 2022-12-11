using UnityEngine;

public class MetroVehicle : Building
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform _body;

    private Transform _player;

    public Transform Body => _body;

    private void Start()
    {
        _player = Player.Presenter.transform;

        _body.transform.localPosition = Vector3.forward * ((Mathf.Abs(_body.position.z - _player.transform.position.z) + Mathf.Abs(_body.position.x - _player.transform.position.x)) * _moveSpeed);
    }


    public void SetSpeedMultiplier(float multiplier)
    {
        _moveSpeed *= multiplier;
    }


    private new void Update()
    {
        if (Game.IsActive == false && _moveSpeed != 0) _moveSpeed = Mathf.MoveTowards( _moveSpeed, 0 , 5 * Time.deltaTime );

        _body.transform.localPosition += Vector3.back * (_moveSpeed * Player.Movement.walkSpeed * Game.Difficulty) * Time.deltaTime;
    }
}