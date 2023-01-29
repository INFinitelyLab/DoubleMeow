using UnityEngine;

public class SpringEffector : MonoBehaviour
{
    private Transform _transform;
    private float _timeRemain = 1f;


    private void Awake()
    {
        _transform = transform;
    }


    public void Play()
    {
        _timeRemain = 0;
    }


    private void Update()
    {
        _transform.localScale = Vector3.one + Vector3.one * Mathf.Sin( _timeRemain * Mathf.PI ) * 0.2f;

        _timeRemain = Mathf.MoveTowards( _timeRemain, 1, 5 * Time.deltaTime );
    }
}
