using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Unbuilder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<Building>(out var building) || other.transform.TryGetComponent<DecorationBuilding>(out var decor) )
        {
            Destroy(other.gameObject);
        }
        else if ( other.gameObject.CompareTag("Vehicle"))
        {
            Destroy(other.transform.parent.gameObject);
        }
    }
}