using UnityEngine;

public class MetroVehicle : Building
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Transform _body;

    private Transform _player;
    private Vector3 _playerPastPosition;
    private Transform _transform2;

    public Transform Body => _body;

    private void Start()
    {
        _transform2 = transform;

        _player = Player.Presenter.transform;
        _playerPastPosition = _player.position;

        _body.transform.localPosition = Vector3.forward * ((Mathf.Abs(_body.position.z - _player.position.z) + Mathf.Abs(_body.position.x - _player.position.x)) * _moveSpeed);
    }


    public void SetSpeedMultiplier(float multiplier)
    {
        _moveSpeed *= multiplier;
    }


    private new void Update()
    {
        if (Game.IsActive && Drone.Instance.IsEnabled == false)  _body.localPosition = Vector3.back * (Quaternion.Inverse(_player.rotation) * (_player.position - _transform2.position)).z * _moveSpeed;

        _playerPastPosition = _player.position;
    }
}