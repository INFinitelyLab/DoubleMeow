using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Pickup : Placeable
{
    public abstract bool IsCanPlace { get; }
    [SerializeField] protected bool DestroyAfterPickup = true;


    private void OnTriggerEnter(Collider other)
    {
        if( other.TryGetComponent<Detector>(out var player) && Drone.Instance.IsEnabled == false && Game.Mode.InInvincibilityMode == false && Game.IsActive)
        {
            OnPickup();

            if (DestroyAfterPickup) Destroy(gameObject);
        }
    }

    protected abstract void OnPickup();

    public abstract void Init();
}