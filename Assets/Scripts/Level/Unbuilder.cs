using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Unbuilder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (Game.PassedTime < 1) return;

        if( other.transform.TryGetComponent<Building>(out var building) || other.transform.TryGetComponent<DecorationBuilding>(out var decor) || other.transform.TryGetComponent<Milk>(out var milk))
        {
            Destroy(other.transform.gameObject);
        }
        else if ( other.gameObject.CompareTag("Vehicle"))
        {
            Destroy(other.transform.parent.gameObject);
        }
    }
}