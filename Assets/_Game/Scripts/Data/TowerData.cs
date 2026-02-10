using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "TowerDefense/Tower Data")]
public class TowerData : ScriptableObject
{
    public string towerName = "Basic Tower";
    public int cost = 50;
    public GameObject prefab; // The 2D sprite prefab with TowerController
}