using _Game.Scripts.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "TowerDefense/Tower Data")]
public class TowerData : ScriptableObject
{
    
    public string towerName = "Basic Tower";
    public int cost = 50;
    public GameObject prefab; // The 2D sprite prefab with TowerController
    [SerializeField] private TowerTypes towerType;
    public TowerTypes TowerType => towerType;

    [Header("AoE Settings")]
    public float aoeRadius = 3f;
    public float damagePerSecond = 5f;
}