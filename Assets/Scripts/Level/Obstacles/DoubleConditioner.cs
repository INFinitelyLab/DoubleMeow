using UnityEngine;

public class DoubleConditioner : MonoBehaviour
{
    [SerializeField] private Transform _forwardRotor;
    [SerializeField] private Transform _backwardRotor;
    [SerializeField] private float _rotateSpeed;


    private void Awake()
    {
        _forwardRotor.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        _backwardRotor.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        _rotateSpeed *= Random.Range(0.5f, 1.5f);
    }


    private void Update()
    {
        _forwardRotor.transform.localRotation *= Quaternion.Euler(0, _rotateSpeed * Time.deltaTime, 0);
        _backwardRotor.transform.localRotation *= Quaternion.Euler(0, _rotateSpeed * Time.deltaTime, 0);
    }
}
