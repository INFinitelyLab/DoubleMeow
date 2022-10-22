using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Accelerator : Placeable
{
    [SerializeField] private byte _length;

    public byte Length => _length;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent<Movement>(out var movement))
        {
            movement.EnableAccelerate();
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent<Movement>(out var movement))
        {
            movement.DisableAccelerate();
        }
    }
}
