using UnityEngine;

public class MetroVehicle : Building
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform _body;

    private Movement _player;

    public Transform Body => _body;

    private void Start()
    {
        _player = Player.Movement;

        _body.transform.localPosition = Vector3.forward * ((Mathf.Abs(_body.position.z - _player.transform.position.z) + Mathf.Abs(_body.position.x - _player.transform.position.x)) * _moveSpeed);
    }


    public void SetSpeedMultiplier(float multiplier)
    {
        _moveSpeed *= multiplier;
    }


    private new void Update()
    {
        if (Game.IsActive == false && _moveSpeed != 0) if (Mathf.Abs(Player.Movement.transform.position.x - transform.position.x) < 0.75f || Game.Mode.InVehicleMode == false) _moveSpeed = Mathf.MoveTowards( _moveSpeed, 0 , 5 * Time.deltaTime );

        _body.transform.localPosition += Vector3.back * (_moveSpeed * _player.walkSpeed * Game.Difficulty) * Time.deltaTime;
    }
}