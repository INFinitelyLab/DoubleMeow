using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _milk;

    private bool _isNeedToMove;

    private Vector3 _targetPosition;

    private void Awake()
    {
        _targetPosition = _body.position;
        _milk.position += Vector3.up * 5;
        _body.position += Vector3.down * 5;
        _body.localEulerAngles = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180) );

        gameObject.SetActive(false);
    }


    private void Update()
    {
        if( _body != null && _milk != null )
        {
            _body.position = Vector3.Lerp(_body.position, _targetPosition, 7.5f * Time.deltaTime );
            _body.rotation = Quaternion.Lerp(_body.rotation, Quaternion.identity, 7.5f * Time.deltaTime );

            _milk.position = Vector3.Lerp(_milk.position, _targetPosition, 7.5f * Time.deltaTime);
        }
    }
}