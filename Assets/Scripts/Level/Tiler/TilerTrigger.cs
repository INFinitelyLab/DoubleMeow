using UnityEngine;

public class TilerTrigger: MonoBehaviour
{
    public bool isNeedToEnableTilerMode;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<Movement>(out var player))
        {
            if (isNeedToEnableTilerMode)
            {
                Player.Camera.EnableTilerMode();
            }
            else
            {
                Player.Camera.DisableTilerMode();
            }
        }
    }
}
