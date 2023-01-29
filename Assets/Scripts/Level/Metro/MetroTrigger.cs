using UnityEngine;

public class MetroTrigger : Trashable
{
    [SerializeField] private bool _isNeedToEnableMetroMode;

    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<Movement>(out var player))
        {
            if (_isNeedToEnableMetroMode)
            {
                if( Drone.Instance.IsEnabled == false )
                {
                    Player.Camera.EnableMetroMode();

                    Invoke(nameof(EnableMetroMode), 0.3f);
                }
            }
            else
            {
                if ( Drone.Instance.IsEnabled == false || Drone.Instance.IsTranslateFromMode == true)
                {
                    Player.Camera.DisableMetroMode( Drone.Instance.IsTranslateFromMode == false );
                    
                    Invoke(nameof(DisableMetroMode), 0.5f); 
                }
                
            }
        }
    }

    private void EnableMetroMode()
    {
        Game.Mode.EnableVehicleMode();

        Fog.Instance.gameObject.SetActive(false);
    }


    private void DisableMetroMode()
    {
        Game.Mode.DisableVehicleMode();
    }
}