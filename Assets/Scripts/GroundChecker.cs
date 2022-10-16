using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool IsGrounded
    {
        get
        {
            return Physics.OverlapBox( transform.position + Vector3.down * 0.1f, new Vector3( 0.1f, 0.1f, 0.1f ), Quaternion.identity, ~LayerMask.GetMask("Player") ).Length > 0;
        }
    }
}