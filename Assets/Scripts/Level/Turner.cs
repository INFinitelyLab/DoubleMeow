using UnityEngine;

public class Turner : MonoBehaviour
{
    [field:SerializeField] public Transform StartPoint { get; private set; }
    [field:SerializeField] public Transform EndPoint { get; private set; }
    [field:SerializeField] public Vector3 ToCenterDirection { get; private set; }
    [field:SerializeField] public Direction Direction { get; private set; }
    [field:SerializeField] public Transform Offset { get; private set; }


    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent<Movement>(out var player))
        {
            player.Turn( this );
        }
    }
}