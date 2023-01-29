using UnityEngine;

public class Disco : MonoBehaviour
{
    [SerializeField] private Transform _ball;
    [SerializeField] private float _rotateSpeed;


    private void Awake()
    {
        _ball.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        _rotateSpeed *= Random.Range(0.5f, 1);
    }


    private void Update()
    {
        _ball.localRotation *= Quaternion.Euler(0, _rotateSpeed * Time.deltaTime, 0);
    }
}
