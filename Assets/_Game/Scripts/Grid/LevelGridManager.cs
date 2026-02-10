using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGridManager : MonoBehaviour
{
    public static LevelGridManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private List<TileData> _tileDatas;
    [Header("Tower Logic")]
    [SerializeField] private GameObject _towerPrefab;

    private Dictionary<Vector2Int, GridNode> _grid = new Dictionary<Vector2Int, GridNode>();
    private Dictionary<TileBase, TileData> _dataLookup = new Dictionary<TileBase, TileData>();

    private void Awake()
    {
        // Singleton Setup
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        InitializeLookup();
        GenerateGridFromTilemap();
    }

    private void InitializeLookup()
    {
        _dataLookup.Clear();

        foreach (var data in _tileDatas)
        {
            foreach (var tile in data.tiles)
            {
                if (_dataLookup.ContainsKey(tile))
                {
                    // FOUND THE BUG!
                    // This prints: "ERROR: Tile [RoadSprite] is defined in BOTH [GrassData] and [PathData]!"
                    Debug.LogError($"DATA COLLISION: The tile '{tile.name}' is trying to be in '{data.name}', but it is ALREADY defined in '{_dataLookup[tile].name}'!");
                }
                else
                {
                    _dataLookup.Add(tile, data);
                }
            }
        }
    }

    private void GenerateGridFromTilemap()
    {
        _grid.Clear();

        // Scan the Tilemap bounds
        BoundsInt bounds = _groundTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cellPos = new Vector3Int(x, y, 0);

                // Only create a node if there is actually a tile there
                if (_groundTilemap.HasTile(cellPos))
                {
                    TileBase tileBase = _groundTilemap.GetTile(cellPos);

                    // --- ADD THIS DEBUG LINE ---
                    if (!_dataLookup.ContainsKey(tileBase))
                    {
                        Debug.LogWarning($"IGNORED TILE: Found '{tileBase.name}' at {cellPos}, but it is not in any ScriptableObject!");
                    }
                    // ---------------------------

                    // Default to non-walkable/non-buildable if data is missing
                    TileData data = _dataLookup.ContainsKey(tileBase) ? _dataLookup[tileBase] : null;

                    if (data != null)
                    {
                        Vector2Int gridPos = new Vector2Int(x, y);
                        Vector3 worldPos = _groundTilemap.GetCellCenterWorld(cellPos);

                        _grid.Add(gridPos, new GridNode(gridPos, worldPos, data));
                    }
                }
            }
        }

        Debug.Log($"Grid Generated: {_grid.Count} nodes.");
    }

    // --- Public API ---

    public GridNode GetNode(Vector2Int gridPos)
    {
        return _grid.ContainsKey(gridPos) ? _grid[gridPos] : null;
    }

    public GridNode GetNodeAtWorldPosition(Vector3 worldPosition)
    {
        Vector3Int cellPos = _groundTilemap.WorldToCell(worldPosition);
        return GetNode(new Vector2Int(cellPos.x, cellPos.y));
    }
    
    private void OnDrawGizmos()
    {
        if (_grid == null) return;

        foreach (var node in _grid.Values)
        {
            // Green for Buildable, Red for Non-Buildable (Path)
            Gizmos.color = node.Data.isBuildable ? new Color(0, 1, 0, 0.3f) : new Color(1, 0, 0, 0.3f);

            // Draw a cube at that node's position (slightly smaller than 1x1 to see gaps)
            Gizmos.DrawCube(node.WorldPosition, Vector3.one * 0.9f);

            // Draw the wireframe outline
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(node.WorldPosition, Vector3.one * 0.9f);
        }
    }

    public bool TryPlaceTower(Vector2Int gridPos)
    {
        // 1. Check if the node exists in our dictionary
        if (!_grid.ContainsKey(gridPos))
        {
            Debug.Log("Invalid Grid Position");
            return false;
        }

        GridNode node = _grid[gridPos];

        // 2. Check the Rules (Is it buildable? Is it already empty?)
        if (node.Data.isBuildable && !node.IsOccupied)
        {
            // 3. Update the Data
            node.IsOccupied = true; // Mark it as taken so we can't build here again

            // 4. Spawn the Visuals
            // We force Z to 0 so it sits perfectly on the 2D plane
            Vector3 spawnPos = new Vector3(node.WorldPosition.x, node.WorldPosition.y, 0);

            Instantiate(_towerPrefab, spawnPos, Quaternion.identity);

            Debug.Log($"Built tower at {gridPos}");
            return true;
        }

        Debug.Log("Cannot build here! (Not buildable or already occupied)");
        return false;
    }
}