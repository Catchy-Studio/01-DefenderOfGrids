using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "TowerDefense/Wave Data")]
public class WaveData : ScriptableObject
{
    [System.Serializable]
    public struct EnemyGroup
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnRate; // Seconds between spawns
    }

    public EnemyGroup[] groups; // A wave can have multiple groups (e.g., 5 Weak, then 1 Strong)
}