using UnityEngine;

public class DecorationBuildingFan : MonoBehaviour
{
    [SerializeField] private Transform _rotor;
    [SerializeField] private float _rotateSpeed = 300;


    private void Update()
    {
        _rotor.rotation *= Quaternion.Euler( 0, 0, _rotateSpeed * Time.deltaTime );
    }
}
