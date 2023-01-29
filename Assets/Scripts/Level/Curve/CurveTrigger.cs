using UnityEngine;

public class CurveTrigger : Trashable
{
    [SerializeField] private bool _isNeedToEnableMetroMode;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<Movement>(out var player))
        {
            if (_isNeedToEnableMetroMode)
            {
                Game.Mode.EnableCurveMode();
            }
            else
            {
                Game.Mode.DisableCurveMode();
            }
        }
    }
}
