using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGridManager : MonoBehaviour
{
    public static LevelGridManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private List<TileData> _tileDatas;
    [SerializeField] private TowerOptionsUI _optionsUI;
    [Header("Tower Logic")]
    [SerializeField] private TowerData _selectedTowerData;

    private Dictionary<Vector2Int, GridNode> _grid = new Dictionary<Vector2Int, GridNode>();
    private Dictionary<TileBase, TileData> _dataLookup = new Dictionary<TileBase, TileData>();
    public event Action<TowerData> OnTowerSelected;
    private GridNode _selectedNode;

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
        // 1. Check if the node exists
        if (!_grid.ContainsKey(gridPos)) return false;
        GridNode node = _grid[gridPos];

        // 2. Check the Rules (Is it buildable? Is it already empty?)
        if (node.Data.isBuildable && !node.IsOccupied)
        {
            // 3. CHECK GOLD
            if (CurrencySystem.Instance.TrySpendGold(_selectedTowerData.cost))
            {
                // Success! We paid. Now build.
                node.IsOccupied = true;

                // Calculate position
                Vector3 spawnPos = new Vector3(node.WorldPosition.x, node.WorldPosition.y, 0);

                // Spawn the tower
                GameObject towerObj = Instantiate(_selectedTowerData.prefab, spawnPos, Quaternion.identity);

                // --- PASS THE COST TO THE TOWER ---
                TowerController controller = towerObj.GetComponent<TowerController>();
                if (controller != null)
                {
                    controller.BaseCost = _selectedTowerData.cost;
                    controller.TowerName = _selectedTowerData.towerName;
                }
                // ----------------------------------

                //Debug.Log($"Built tower! Gold Remaining: {CurrencySystem.Instance.CurrentGold}");
                return true;
            }
        }

        return false;
    }

    // Add this anywhere in the class (maybe near the bottom with other Public APIs)
    public void SelectTower(TowerData data)
    {
        _selectedTowerData = data;
        if (_optionsUI != null)
        {
            _optionsUI.Hide();
        }
        OnTowerSelected?.Invoke(data);
        Debug.Log($"Selected Tower: {data.towerName}");
    }

    public void SellTower(TowerController tower)
    {
        // 1. Find the Grid Node under this tower
        GridNode node = GetNodeAtWorldPosition(tower.transform.position);

        if (node != null)
        {
            node.IsOccupied = false; // <--- CRITICAL FIX: Mark the spot as free!
            Debug.Log($"Freed up node at {node.GridPosition}");
        }

        // 2. Refund Gold
        int refundAmount = tower.GetSellValue();
        CurrencySystem.Instance.AddGold(refundAmount);

        // 3. Destroy the Tower
        Destroy(tower.gameObject);
    }

    public void HandleGridClick(Vector2Int gridPos)
    {
        if (!_grid.ContainsKey(gridPos)) return;
        GridNode clickedNode = _grid[gridPos];

        // CASE A: The Node has a Tower -> SELECT IT
        if (clickedNode.IsOccupied)
        {
            // Find the tower object at that position (Physics check is easiest here)
            Collider2D[] hits = Physics2D.OverlapPointAll(clickedNode.WorldPosition);
            foreach (var hit in hits)
            {
                TowerController tower = hit.GetComponent<TowerController>();
                if (tower != null)
                {
                    _optionsUI.Show(tower); // <--- OPEN THE MENU
                    _selectedNode = clickedNode;
                    return;
                }
            }
            // TODO: Open Upgrade UI here later
        }
        // CASE B: We clicked something else (Grass, Path, Empty space)
        // 1. Always Close the UI first
        _optionsUI.Hide();
        _selectedNode = null;

        // 2. Now check if we wanted to BUILD something new
        if (clickedNode.Data.isBuildable)
        {
            if (_selectedTowerData != null)
            {
                TryPlaceTower(gridPos);
            }
        }
    }
}