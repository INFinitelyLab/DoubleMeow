using UnityEngine;

public abstract class ObstaclerBase : MonoBehaviour
{
    public class Cell
    {
        public bool IsEmpty => Placeable == null;
        public Placeable Placeable { get; private set; }

        public Cell(Placeable placeable)
        {
            Placeable = placeable;
        }
    }


    public class LargeCell
    {
        public Cell[] Cells { get; private set; }

        public bool IsPath { get; private set; }
        public bool IsObstacle { get; private set; }
        public bool IsEmpty { get; private set; } = true;


        public LargeCell()
        {
            Cells = new Cell[3];

            IsPath = false;
        }

        public void EnablePath()
        {
            IsPath = true;
        }


        public void EnableObstacle()
        {
            IsObstacle = true;
        }

        public void AddPlaceable(Placeable placeable, int depth)
        {
            if (depth < 0 && depth > 2) return;

            IsEmpty = false;

            Cells[depth] = new Cell(placeable);
        }
    }


    public enum PortalPosition
    {
        Forward,
        None,
        Backward
    }
}
