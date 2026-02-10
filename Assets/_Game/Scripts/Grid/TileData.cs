using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "NewTileData", menuName = "TowerDefense/Grid/TileData")]
public class TileData : ScriptableObject
{
    [Header("Visuals")]
    public TileBase[] tiles; // Drag the palette tiles here (e.g., GrassTile)

    [Header("Logic")]
    public bool isWalkable;
    public bool isBuildable;
    public float movementCost = 1f; // Future-proofing: 1 = normal, 2 = mud/slow
}