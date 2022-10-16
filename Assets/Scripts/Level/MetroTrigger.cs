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
                Game.Mode.EnableVehicleMode();

                Fog.Instance.gameObject.SetActive(false);
            }
            else
            {
                Game.Mode.DisableVehicleMode();
            }
        }
    }
}