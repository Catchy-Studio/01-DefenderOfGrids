using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _pathParent; // Drag the "Path" object here
    [SerializeField] private float _spawnInterval = 2f;

    private Transform[] _waypoints;
    private float _timer;

    private void Start()
    {
        // Get all the child waypoints automatically
        _waypoints = new Transform[_pathParent.childCount];
        for (int i = 0; i < _pathParent.childCount; i++)
        {
            _waypoints[i] = _pathParent.GetChild(i);
        }
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= _spawnInterval)
        {
            SpawnEnemy();
            _timer = 0;
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemyObj = Instantiate(_enemyPrefab);

        // Get the script and give it the path
        EnemyMovement movement = enemyObj.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.Initialize(_waypoints);
        }
    }
}