using UnityEngine;

public class MetroTrigger : MonoBehaviour
{
    [SerializeField] private bool _isNeedToEnableMetroMode;

    private void OnTriggerEnter(Collider other)
    {
        if( other.transform.TryGetComponent<Movement>(out var player) )
        {
            if (_isNeedToEnableMetroMode)
            {
                Player.Camera.EnableMetroMode();

                Invoke(nameof(EnableMetroMode), 0.3f);
            }
            else
            {
                Player.Camera.DisableMetroMode();

                Invoke(nameof(DisableMetroMode), 0.5f);
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