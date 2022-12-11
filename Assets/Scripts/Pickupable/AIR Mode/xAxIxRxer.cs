using UnityEngine;
using System.Collections;

public class xAxIxRxer : SingleBehaviour<xAxIxRxer>
{
    [SerializeField] private xAxIxRxTrigger _trigger;
    [SerializeField] private Animator _animator;
    [SerializeField] private float _targetDistance = 50;
    [SerializeField] private Milk _milk;

    private static Transform _player;
    private static Transform _camera;

    private static Transform _playerParent;
    private static Transform _cameraParent;

    private static float _intensity;

    private Vector3 _generatePosition;


    public static void Enable()
    {
        Instance.StartCoroutine(Instance.Generate());

        Player.Movement.enabled = false;
        Player.Camera.enabled = false;

        _playerParent = Instance.transform.GetChild(0);
        _cameraParent = Instance.transform.GetChild(1);

        Instance.transform.position = Player.Movement.transform.position;
        Instance.transform.rotation = Player.Movement.transform.rotation;
        _playerParent.localPosition = Vector3.zero;
        _cameraParent.localPosition = Vector3.zero;

        _player = Player.Movement.transform;
        _camera = Player.Camera.transform;

        Instance.transform.localScale = new Vector3(1, (7 - Player.Movement.transform.position.y) / 7, 1); ;
        _playerParent.localScale = new Vector3(1, 7 / (7 - Player.Movement.transform.position.y), 1);
        _cameraParent.localScale = new Vector3(1, 7 / (7 - Player.Movement.transform.position.y), 1);

        _player.SetParent(_playerParent);
        _camera.SetParent(_cameraParent);

        Instance.StartCoroutine(Instance.AnimateIn());
    }

    public static void Disable()
    {
        Player.Movement.enabled = false;
        Player.Camera.enabled = false;

        Instance.transform.position = _player.transform.position;
        Instance.transform.rotation = _player.transform.rotation;
        _playerParent.localPosition = Vector3.zero;
        _cameraParent.localPosition = Vector3.zero;

        _player.SetParent(_playerParent);
        _camera.SetParent(_cameraParent);

        Game.Mode.DisablexAxIxRxMode();
        Game.Mode.EnableParachuteMode();

        Instance.StartCoroutine(Instance.AnimateOut());
    }


    private void Update()
    {
        if (Game.Mode.InxAxIxRxMode)
        {
            Vector3 position = _camera.localPosition;
            position.x = Mathf.Lerp( position.x, 0, Time.deltaTime );

            _camera.localPosition = position;
        }
    }


    private IEnumerator Generate()
    {
        Quaternion rotation = Player.Movement.transform.rotation;
        Vector3 startPosition = Player.Movement.TurnPositionWithoutRotation + (rotation * (Vector3.forward * Player.Movement.Position.z));

        Vector3 position = Vector3.up * 7;

        bool isLeft = true;
        float distance = 10;
        float newPoint = 0;
        Vector2 point = new Vector2(Random.Range(0.8f, 0.8f), distance);

        while(distance < _targetDistance)
        {
            if(point.y <= distance)
            {
                isLeft = !isLeft;

                newPoint = Random.Range(0.3f, 1.6f) * (isLeft? 1 : -1);

                if (newPoint + point.x > 1.3f || newPoint + point.x < -1.3f)
                    newPoint *= -1;

                point = new Vector2( Mathf.Clamp(point.x + newPoint, -1.3f, 1.3f) , distance + 3);
            }

            position.x = Mathf.MoveTowards(position.x, point.x, 0.746f / 3 );
            position.z = distance;

            CreateMilk(startPosition + rotation * position);

            distance += 0.746f;

            yield return new WaitForFixedUpdate();
        }

        _generatePosition = startPosition + rotation * Vector3.forward * distance;
        _generatePosition.y = 0;

        Instantiate( _trigger, _generatePosition + Vector3.up * 7, rotation );
    } 


    private IEnumerator AnimateIn()
    {
        _animator.SetTrigger("Play");

        yield return new WaitForFixedUpdate();

        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil( () => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        _player.parent = null;
        _camera.parent = null;

        yield return new WaitForFixedUpdate();

        Player.Movement.enabled = true;
        Player.Camera.enabled = true;

        Player.Camera.EnablexAxIxRxMode();
    }

    private IEnumerator AnimateOut()
    {
        _animator.SetTrigger("PlayOut");

        yield return new WaitForFixedUpdate();


        yield return new WaitForSeconds(0.5f);

        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1);

        _player.parent = null;
        _camera.parent = null;

        yield return new WaitForFixedUpdate();

        Player.Movement.enabled = true;
        Player.Camera.enabled = true;
    }


    private static void CreateMilk(Vector3 position)
    {
        Instantiate(Instance._milk, position, Quaternion.identity);
    }
}
