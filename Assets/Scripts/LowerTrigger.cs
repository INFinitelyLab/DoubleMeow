using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LowerTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<ILowereable>(out var lowereable) )
        {
            lowereable.Low(0.7f);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.transform.TryGetComponent<ILowereable>(out var lowereable))
        {
            lowereable.Up();
        }
    }
}