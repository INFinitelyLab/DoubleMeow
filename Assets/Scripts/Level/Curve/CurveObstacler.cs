using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class CurveObstacler : ObstaclerBase
{
    [SerializeField] protected SolarPanel dynamicSolarPanelPrefab;
    [SerializeField] protected Milk milk;
    [SerializeField] private Vector2 _cellSize;

    private static CurveObstacler _obstacler;

    protected Cell[,] Cells;


    public int Generate(Curve curve, int startIntLine)
    {
        Vector2Int size = curve.GetSize();

        Cells = new Cell[size.x, size.y];

        //GenerateSolarPanels(curve);
        GenerateMilk(curve, startIntLine, out int EndIntLine);

        return EndIntLine;
    }


    private void GenerateSolarPanels(Curve curve)
    {
        int length = curve.PositionsForSolarPanels.Length;

        for (int index = 0; index < length; index++)
        {
            Transform point = curve.PositionsForSolarPanels[index];

            CreatePlaceable( dynamicSolarPanelPrefab, Vector3.zero, point );
        }
    }

    private void GenerateMilk(Curve curve, int startIntLine, out int EndIntLine)
    {
        Vector2Int positionInt = new Vector2Int();
        Vector2Int size = curve.GetSize();

        List<int> isEmptyTile = new List<int>(5);

        int line = startIntLine;

        for( int y = 0; y < size.y; y++ )
        {
            /// === Check for surf line === //

            positionInt.x = line + 2;
            positionInt.y = y;
            if ( curve.GetTileID( positionInt ) == 0 )
            {
                for(int x = 0; x < 5; x++)
                {
                    positionInt.x = x;

                    if (curve.GetTileID(positionInt) > 0)
                        isEmptyTile.Add(x - 2);
                }

                line = isEmptyTile.Random();

                isEmptyTile.Clear();
            }

            Vector3 position = new Vector3((int)line * -_cellSize.x * 0.8f, Mathf.Pow( Mathf.Abs(line) / 1.2f + 0.001f, 1.5f ) / 4 - 0.01f, y * _cellSize.y + (_cellSize.y / 4));

            if((y) % 6 != 0 && (y+2) % 6 != 0)
                CreatePlaceable(milk, position, curve.transform);
        }

        EndIntLine = line;
    }


    private void OldGenerateMilk(Curve curve)
    {
        int count = curve.PositionsForSolarPanels.Length;

        for (int index = 0; index < count; index++)
        {
            Vector2Int xy = new Vector2Int( Mathf.RoundToInt(curve.PositionsForSolarPanels[index].localPosition.x * 1.5f), Mathf.RoundToInt((curve.PositionsForSolarPanels[index].localPosition.z - 0.5f) / 0.746f) - 1 );

            if (xy.y < 4) continue;

            int length = Random.Range(3, 7);

            for( int j = 0; j < length; j++ )
            {
                if (curve.GetTileID(new Vector2Int(xy.x + 2, xy.y)) == 0) break;

                float x = Mathf.Pow(Mathf.Abs(xy.x), 0.65f) * (xy.x < 0? -0.8f : 0.8f);

                Vector3 position = new Vector3( x == float.NaN? 0 : x, 1.1f - Mathf.Cos(xy.x / 1.05f / 1.65f), xy.y * 0.746f + 0.5f);

                Placeable placeable = CreatePlaceable(milk, position, curve.transform);

                placeable.transform.localRotation = Quaternion.Euler(0,0, xy.x * 25);

                Cells[xy.x + 2, xy.y] = new Cell(placeable);

                xy.y--;
            }

            index += Random.Range(0, 3);
        }
    }


    protected Placeable CreatePlaceable(Placeable origin, Vector3 position, Transform parent, bool isInverse = false)
    {
        Placeable placeable = Instantiate(origin, parent);

        placeable.transform.localPosition = position;
        placeable.transform.localRotation = Quaternion.Euler(0, isInverse ? 180 : 0, 0);

        return placeable;
    }
}
