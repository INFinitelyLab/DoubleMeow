using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _milk;

    private bool _isNeedToMove;

    private Vector3 _bodyTargetPosition;
    private Vector3 _milkTargetPosition;

    private void Awake()
    {
        _bodyTargetPosition = _body.localPosition;
        _milkTargetPosition = _milk.localPosition;
        _milk.localPosition += Vector3.up * 5;
        _body.localPosition += Vector3.down * 5;
        _body.localEulerAngles = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180) );

        gameObject.SetActive(false);
    }

    public void Endplace()
    {
        _body.localPosition = _bodyTargetPosition;
        _body.localRotation = Quaternion.identity;

        _milk.localPosition = _milkTargetPosition;
    }


    private void Update()
    {
        if( _body != null && _milk != null )
        {
            _body.localPosition = Vector3.Lerp(_body.localPosition, _bodyTargetPosition, 7.5f * Time.deltaTime );
            _body.localRotation = Quaternion.Lerp(_body.localRotation, Quaternion.identity, 7.5f * Time.deltaTime );

            _milk.localPosition = Vector3.Lerp(_milk.localPosition, _milkTargetPosition, 7.5f * Time.deltaTime);
        }
    }
}