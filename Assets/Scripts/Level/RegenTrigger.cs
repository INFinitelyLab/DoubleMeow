using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RegenTrigger : SingleBehaviour<RegenTrigger>
{
    public System.Action Triggered;


    private void OnTriggerEnter(Collider other)
    {
        if( other.TryGetComponent<Player>(out var player) )
        {
            Triggered?.Invoke();
        }
    }


    public void MoveTo(Vector3 position)
    {
        transform.localPosition = position;
    }
}