using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Pickup : Placeable
{
    public abstract bool IsCanPlace { get; }


    private void OnTriggerEnter(Collider other)
    {
        if( other.TryGetComponent<Player>(out var player) )
        {
            OnPickup();

            Destroy(gameObject);
        }
    }

    protected abstract void OnPickup();

}