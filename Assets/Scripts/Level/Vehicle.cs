using UnityEngine;

public class Vehicle : Building
{
    [SerializeField] private Transform _body;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _moveIntensive;

    private static bool _isGlobalLeft = false;

    private bool _isLeft = false;

    private Movement _player;


    private void Start()
    {
        _isLeft = !_isGlobalLeft;
        _isGlobalLeft = _isLeft;

        if (_isLeft == false)
        {
            _body.localScale = new Vector3(-1, 1, 1);
        }

        _player = Player.Movement;

        Vector3 position = _body.localPosition;

        position.x = (_player.transform.position.z - transform.position.z) * _moveIntensive - 0.15f;
        position.x *= _isLeft ? 1 : -1;

        _body.localPosition = position;
    }


    private void FixedUpdate()
    {
        _body.localPosition += (_isLeft? Vector3.right : Vector3.left) * Player.Movement.walkSpeed * Game.Difficulty * _moveIntensive * Time.fixedDeltaTime;
    }


    private void OnTriggerEnter(Collider collision)
    {
        Debug.Log("FDYBVu");

        if (collision.transform.TryGetComponent<Player>(out var player))
        {
            _animator.Play("Vehicle_Bounce");
        }
    }


    public override void Animate() { }

    protected override void SpawnBird() { }
}