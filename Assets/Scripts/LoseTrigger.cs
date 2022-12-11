using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LoseTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<Player>(out var player))
        {
            if(Game.IsActive && Drone.Instance.IsEnabled == false)
            {
                Game.Stop(true);
            }
            
            Player.Movement.enabled = false;
            Player.Movement.transform.position -= Vector3.up * 2;
        }

    }
}
