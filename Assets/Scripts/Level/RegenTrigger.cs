using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RegenTrigger : MonoBehaviour
{
    public System.Action Triggered;


    private void OnTriggerEnter(Collider other)
    {
        if( other.TryGetComponent<Player>(out var player) )
        {
            Triggered?.Invoke();
        }
    }


    public void MoveTo(float distance)
    {
        transform.localPosition = Vector3.forward * (distance + 3);
    }
}