using UnityEngine;

public class Vehicle : Building
{
    [SerializeField] private Transform _body;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveIntensive;

    private static bool _isGlobalLeft = false;

    private bool _isLeft = false;

    private Vector3 _offset = new Vector3(0f, -0.5f, 0.5f);

    private Movement _player;

    private Transform _playerTransform;
    private Transform _transform2;

    private void Start()
    {
        _isLeft = !_isGlobalLeft;
        _isGlobalLeft = _isLeft;

        if (_isLeft == false)
        {
            _body.localScale = new Vector3(-1, 1, 1);
        }

        _player = Player.Movement;
        _playerTransform = _player.transform;
        _transform2 = transform;

        Vector3 position = _body.localPosition;

        position.x = (_player.transform.position.z - transform.position.z) * _moveIntensive - 0.15f;
        position.x *= _isLeft ? 1 : -1;

        _body.localPosition = position;
    }


    protected void LateUpdate()
    {
        _body.localPosition = ((_isLeft ? Vector3.right : Vector3.left) * (_playerTransform.position.z - _transform2.position.z)) * _moveIntensive + _offset;
    }


    protected override void Update()
    {
       //
    }


    public override void Animate() { }

    protected override void SpawnBird() { }
}