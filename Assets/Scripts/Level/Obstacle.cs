using UnityEngine;

public class Obstacle : Placeable, IGroundeable
{
    [SerializeField, Multiline] private string _tiles;
    [SerializeField] private Vector2Int _size;

    public string Tiles => _tiles;
    public Vector2Int Size => _size;
    public bool[,] IsEmpty;

    public void Initialize()
    {
        IsEmpty = new bool[_size.x, _size.y];

        string[] lines = _tiles.Split("\n");

        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                IsEmpty[x, y] = lines[y][x] - 48 == 1;
            }
        }
    }


    public virtual bool IsCanPlaceHere(Building building, ObstaclerBase.LargeCell[,] depthTiles, int x, int y, Obstacler.PortalPosition portalPosition) { return true; }
}


public abstract class Placeable : MonoBehaviour
{
    [SerializeField] private int _chance;

    public int chance => _chance;
}