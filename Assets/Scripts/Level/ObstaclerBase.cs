using UnityEngine;

public abstract class ObstaclerBase : MonoBehaviour
{
    protected class Cell
    {
        public bool IsEmpty => Placeable == null;
        public Placeable Placeable { get; private set; }

        public Cell(Placeable placeable)
        {
            Placeable = placeable;
        }
    }


    protected class LargeCell
    {
        public Cell[] Cells { get; private set; }

        public bool IsPath { get; private set; }
        public bool IsEmpty => Cells[0].IsEmpty == false && Cells[1].IsEmpty == false && Cells[2].IsEmpty == false;


        public LargeCell()
        {
            Cells = new Cell[3];

            IsPath = false;
        }

        public void EnablePath() => IsPath = true;

        public void AddPlaceable(Placeable placeable, int depth)
        {
            if (depth < 0 && depth > 2) return;

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
