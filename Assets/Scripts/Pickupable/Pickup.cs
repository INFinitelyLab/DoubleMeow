using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Pickup : Placeable
{
    public abstract bool IsCanPlace { get; }
    [SerializeField] protected bool DestroyAfterPickup = true;


    private void OnTriggerEnter(Collider other)
    {
        if( other.TryGetComponent<Player>(out var player) )
        {
            OnPickup();

            if (DestroyAfterPickup) Destroy(gameObject);
        }
    }

    protected abstract void OnPickup();

}