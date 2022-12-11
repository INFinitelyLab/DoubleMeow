using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Pickup : Placeable
{
    public abstract bool IsCanPlace { get; }
    [SerializeField] protected bool DestroyAfterPickup = true;


    private void OnTriggerEnter(Collider other)
    {
        if( other.TryGetComponent<Player>(out var player) && Game.Mode.InInvincibilityMode == false )
        {
            OnPickup();

            if (DestroyAfterPickup) Destroy(gameObject);
        }
    }

    protected abstract void OnPickup();

    public abstract void Init();
}