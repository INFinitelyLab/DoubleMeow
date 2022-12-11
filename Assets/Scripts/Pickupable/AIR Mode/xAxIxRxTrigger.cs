using UnityEngine;

[RequireComponent(typeof(Collider))]
public class xAxIxRxTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent<Player>(out var player))
        {
            xAxIxRxer.Disable();

            Destroy(gameObject);
        }
    }
}
