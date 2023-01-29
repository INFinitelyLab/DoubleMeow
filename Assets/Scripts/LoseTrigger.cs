using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LoseTrigger : Trashable
{
    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<Player>(out var player) && Drone.Instance.IsEnabled == false)
        {
            if(Game.IsActive)
            {
                if (Game.Mode.InParachuteMode == true) Player.Camera.enabled = false;

                Game.Stop(true);
            }
            
            //Player.Movement.enabled = false;
            //Player.Movement.transform.position -= Vector3.up * 2;
        }

    }
}
