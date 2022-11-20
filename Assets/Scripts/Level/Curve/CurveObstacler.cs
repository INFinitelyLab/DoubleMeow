using System.Collections;
using UnityEngine;

public class CurveObstacler : ObstaclerBase
{
    [SerializeField] protected SolarPanel dynamicSolarPanelPrefab;
    [SerializeField] protected Milk milk;
    [SerializeField] private Vector2 _cellSize;

    private static CurveObstacler _obstacler;

    protected Cell[,] Cells;


    public RoadLine Generate(Curve curve, RoadLine startLine)
    {
        Vector2Int size = curve.GetSize();

        Cells = new Cell[size.x, size.y];

        GenerateSolarPanels(curve);
        GenerateMilk(curve, startLine, out RoadLine endLine);

        return endLine;
    }


    private void GenerateSolarPanels(Curve curve)
    {
        int length = curve.PositionsForSolarPanels.Length;

        for (int index = 0; index < length; index++)
        {
            Transform point = curve.PositionsForSolarPanels[index];

            if (Mathf.Abs(point.localPosition.x) > 0.1f || Random.Range(0, 2) == 0)
                CreatePlaceable( dynamicSolarPanelPrefab, Vector3.zero, point );
        }
    }

    private void GenerateMilk(Curve curve, RoadLine startLine, out RoadLine endLine)
    {
        Vector2Int size = curve.GetSize();

        RoadLine line = startLine;

        for( int y = 0; y < size.y; y++ )
        {
            /// === Check for surf line === //
            if( curve.GetTileID( new Vector2Int( (int)line + 1, y ) ) == 0 )
            {
                int surfDirections = 0;

                if (curve.GetTileID(new Vector2Int((int)line, y)) == 1)
                    surfDirections += 1;
                if (curve.GetTileID(new Vector2Int((int)line + 2, y)) == 1)
                    surfDirections += 2;

                if (surfDirections == 3)
                    line.TrySurfRandom();
                else
                    line.TrySurf( surfDirections == 1? Direction.Left : Direction.Right );
            }

            Vector3 position = new Vector3((int)line * -_cellSize.x * 0.7f, line == RoadLine.Venus ? 0 : 0.4f, y * _cellSize.y + (_cellSize.y / 4));

            CreatePlaceable(milk, position - Vector3.forward * 0.25f, curve.transform);
            CreatePlaceable(milk, position + Vector3.forward * 0.25f, curve.transform);
        }

        endLine = line;
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
