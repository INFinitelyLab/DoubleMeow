using UnityEngine;

public class SolarPanel : Placeable
{
    private Vector3 targetPosition;
    private bool isActive;


    private void Start()
    {
        targetPosition = Vector3.up * 0.3f;
    }


    private void Update()
    {
        if(isActive) transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, 5 * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<Player>(out var player) )
        {
            isActive = true;
        }
    }
}
