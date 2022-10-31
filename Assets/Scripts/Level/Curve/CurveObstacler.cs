using UnityEngine;

public class CurveObstacler : ObstaclerBase
{
    [SerializeField] protected SolarPanel dynamicSolarPanelPrefab;
    [SerializeField] protected Milk milk;

    protected Cell[,] Cells;


    public RoadLine Generate(Curve curve, RoadLine startLine)
    {
        RoadLine endLine = startLine;

        Vector2Int size = curve.GetSize();

        Cells = new Cell[size.x, size.y];

        Debug.Log("Size : " + size);

        GenerateSolarPanels(curve);
        GenerateMilk(curve);

        return endLine;
    }


    private void GenerateSolarPanels(Curve curve)
    {
        int iterations = Random.Range(0, 3);


        for(int index = 0; index < iterations; index++)
        {
            Transform point = curve.PositionsForSolarPanels.Random();

            Vector2Int position = new Vector2Int( Mathf.RoundToInt(point.transform.localPosition.x * 1.5f), Mathf.RoundToInt(point.transform.localPosition.z / 0.746f) );

            if(Cells[position.x + 2, position.y] == null)
            {
                Cells[position.x + 2, position.y] = new Cell(CreatePlaceable( dynamicSolarPanelPrefab, Vector3.zero, point ));
            }
        }

        int Length = curve.PositionsForStaticSolarPanels.Length;

        for(int index = 0; index < Length; index ++)
        {
            Transform point = curve.PositionsForStaticSolarPanels[index];

            Vector2Int position = new Vector2Int(Mathf.RoundToInt(point.transform.localPosition.x * 1.5f), Mathf.RoundToInt(point.transform.localPosition.z / 0.746f));

            Cells[position.x + 2, position.y] = new Cell(CreatePlaceable(dynamicSolarPanelPrefab, Vector3.zero, point));
        }
    }


    private void GenerateMilk(Curve curve)
    {
        int count = curve.PositionsForSolarPanels.Length;

        for (int index = 0; index < count; index++)
        {
            Vector2Int xy = new Vector2Int( Mathf.RoundToInt(curve.PositionsForSolarPanels[index].localPosition.x * 1.5f), Mathf.RoundToInt((curve.PositionsForSolarPanels[index].localPosition.z - 0.5f) / 0.746f) - 1 );

            Debug.Log("Position : " + xy);

            if (xy.y < 3) continue;

            int length = Random.Range(3, 7);

            for( int j = 0; j < length; j++ )
            {
                if (curve.GetTileID(new Vector2Int(xy.x +2, xy.y)) == 0) break;

                Vector3 position = new Vector3(xy.x / 1.25f, 0.7f - Mathf.Cos(xy.x / 1.05f) * 0.5f, xy.y * 0.746f + 0.5f);

                Debug.Log("Position : " + xy);

                Cells[xy.x + 2, xy.y] = new Cell(CreatePlaceable(milk, position, curve.transform));

                xy.y--;
            }

            index += Random.Range(0, 4);
        }

        count = curve.PositionsForStaticSolarPanels.Length;

        for (int index = 0; index < count; index++)
        {
            Vector2Int xy = new Vector2Int(Mathf.RoundToInt(curve.PositionsForStaticSolarPanels[index].localPosition.x * 1.75f), Mathf.RoundToInt((curve.PositionsForStaticSolarPanels[index].localPosition.z - 0.5f) / 0.746f) - 1);

            Debug.Log("Position : " + xy);

            if (xy.y < 3) continue;

            int length = Random.Range(3, 7);

            for (int j = 0; j < length; j++)
            {
                if (curve.GetTileID(new Vector2Int(xy.x + 2, xy.y)) == 0 || Cells[xy.x + 2, xy.y] != null) break;

                Vector3 position = new Vector3(xy.x / 1.5f, 0.7f - Mathf.Cos(xy.x / 1.05f) * 0.5f, xy.y * 0.746f + 0.5f);

                Debug.Log("Position : " + xy);

                Cells[xy.x + 2, xy.y] = new Cell(CreatePlaceable(milk, position, curve.transform));

                xy.y--;
            }

            index += Random.Range(0, 4);
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
