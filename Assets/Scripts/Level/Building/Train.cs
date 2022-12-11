using UnityEngine;

public class Train : Building
{
    [SerializeField] private Transform _rightTrain;
    [SerializeField] private Transform _leftTrain;
    [SerializeField] private float _moveIntensive;

    private Quaternion _rotation;

    private Transform _player;

    private Transform _playerTransform;
    private Transform _transform2;

    private void Start()
    {
        _player = Player.Presenter.transform;
        _playerTransform = _player.transform;
        _transform2 = transform;

        _leftTrain.localPosition = Vector3.right * (((_rotation * _playerTransform.position).z - (_rotation * _transform2.position).z) * _moveIntensive);
        _rightTrain.localPosition = Vector3.left * (((_rotation * _playerTransform.position).z - (_rotation * _transform2.position).z) * _moveIntensive);

        _rotation = Quaternion.Inverse(_transform2.rotation);
    }


    protected void LateUpdate()
    {
        _leftTrain.localPosition = Vector3.right * (((_rotation * _playerTransform.position).z - (_rotation * _transform2.position).z + 5 * Game.Difficulty) * _moveIntensive);
        _rightTrain.localPosition = Vector3.left * (((_rotation * _playerTransform.position).z - (_rotation * _transform2.position).z + 5 * Game.Difficulty) * _moveIntensive);
    }


    protected override void Update()
    {
        //
    }


    public override void Animate() { }

    protected override void SpawnBird() { }
}