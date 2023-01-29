using UnityEngine;

public class RetrowaveDecorator : MonoBehaviour
{
    [SerializeField] private RetroDecobuilding[] _decorationPrefabs;

    public void Decorate(Retrobuilding origin)
    {
        /* Old code
        RetroDecobuilding building = null;

        for( int z = 0; z < 4; z++ )
        {
            building = Instantiate(_decorationPrefabs.Random(), origin.transform);
            building.transform.localPosition = Vector3.forward * (z * 3.735326f - 1.867663f);
        }
        */
    }
}