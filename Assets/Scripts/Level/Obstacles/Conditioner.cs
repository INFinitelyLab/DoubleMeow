using UnityEngine;

public class Conditioner : MonoBehaviour
{
    [SerializeField] private Transform _rotor;
    [SerializeField] private float _rotateSpeed;


    private void Awake()
    {
        _rotor.localRotation = Quaternion.Euler(0,0, Random.Range(0,360) );
        _rotateSpeed *= Random.Range(0.5f, 1.5f);
    }


    private void Update()
    {
        _rotor.localRotation *= Quaternion.Euler(0,0,_rotateSpeed * Time.deltaTime);
    }
}
