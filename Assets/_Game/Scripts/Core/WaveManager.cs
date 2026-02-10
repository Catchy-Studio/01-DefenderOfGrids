using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<WaveData> _waves;
    [SerializeField] private Transform _pathParent;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _waveText;
    [SerializeField] private GameObject _nextWaveButton;

    private int _currentWaveIndex = 0;
    private Transform[] _waypoints;
    private bool _isSpawning = false;

    private void Start()
    {
        // Get Waypoints (same as before)
        _waypoints = new Transform[_pathParent.childCount];
        for (int i = 0; i < _pathParent.childCount; i++)
        {
            _waypoints[i] = _pathParent.GetChild(i);
        }

        UpdateUI();
    }

    // Call this from a UI Button
    public void StartNextWave()
    {
        if (_currentWaveIndex >= _waves.Count)
        {
            Debug.Log("All waves cleared! Level Complete!");
            return;
        }

        StartCoroutine(SpawnWaveRoutine(_waves[_currentWaveIndex]));

        // Hide button while wave is active
        if (_nextWaveButton != null) _nextWaveButton.SetActive(false);
    }

    private IEnumerator SpawnWaveRoutine(WaveData wave)
    {
        _isSpawning = true;

        foreach (var group in wave.groups)
        {
            for (int i = 0; i < group.count; i++)
            {
                SpawnEnemy(group.enemyPrefab);
                yield return new WaitForSeconds(group.spawnRate);
            }
        }

        _isSpawning = false;
        _currentWaveIndex++;

        // Allow starting the next wave
        // In a real game, we would wait for all enemies to die first.
        // For now, let's just show the button again immediately after spawning finishes.
        if (_nextWaveButton != null) _nextWaveButton.SetActive(true);

        UpdateUI();
    }

    private void SpawnEnemy(GameObject prefab)
    {
        GameObject enemy = Instantiate(prefab);
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.Initialize(_waypoints);
        }
    }

    private void UpdateUI()
    {
        if (_waveText != null) _waveText.text = $"Wave: {_currentWaveIndex + 1} / {_waves.Count}";
    }
}
