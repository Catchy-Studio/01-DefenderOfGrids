using UnityEngine;

// Pure C# class - lighter on memory than MonoBehaviour
public class GridNode
{
    public Vector2Int GridPosition { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public TileData Data { get; private set; }

    // State
    public bool IsOccupied { get; set; } // Is a tower here?

    public GridNode(Vector2Int gridPos, Vector3 worldPos, TileData data)
    {
        GridPosition = gridPos;
        WorldPosition = worldPos;
        Data = data;
    }
}