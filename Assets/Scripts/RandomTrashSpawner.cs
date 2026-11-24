using System.Collections.Generic;
using UnityEngine;

public class RandomTrashSpawner : MonoBehaviour
{
    [Header("Items")]
    [SerializeField] private CollectibleTrash[] _prefabs;
    [SerializeField, Min(0)] private int _itemCount = 24;
    [SerializeField, Tooltip("Spawn new trash after collection. Off keeps cleanup progress permanent for this session.")]
    private bool _replenishCollected;
    [SerializeField, Min(0.1f)] private float _replenishInterval = 5f;

    [Header("Spawn Volume (local coordinates)")]
    [SerializeField] private Vector3 _areaSize = new Vector3(24f, 3f, 24f);
    [SerializeField, Min(0f)] private float _minimumSpacing = 1.5f;
    [SerializeField, Min(0f), Tooltip("Radius enclosing the largest trash prefab, including its offset from its pivot.")]
    private float _clearanceRadius = 0.75f;
    [SerializeField] private LayerMask _obstacleLayers = ~0;
    [SerializeField, Min(1)] private int _attemptsPerItem = 40;

    private readonly List<CollectibleTrash> _spawned = new List<CollectibleTrash>();
    private float _nextReplenishTime;

    public event System.Action<CollectibleTrash> ItemSpawned;
    public CollectibleTrash[] GetActiveItems()
    {
        _spawned.RemoveAll(item => item == null || item.IsCollected);
        return _spawned.ToArray();
    }

    public int ActiveCount
    {
        get
        {
            _spawned.RemoveAll(item => item == null || item.IsCollected);
            return _spawned.Count;
        }
    }

    private void Start()
    {
        SpawnMissing();
        _nextReplenishTime = Time.time + _replenishInterval;
    }

    private void Update()
    {
        if (!_replenishCollected || Time.time < _nextReplenishTime) return;
        _nextReplenishTime = Time.time + _replenishInterval;
        SpawnMissing();
    }

    public int SpawnMissing()
    {
        if (_prefabs == null || _prefabs.Length == 0)
        {
            Debug.LogWarning("RandomTrashSpawner needs collectible prefabs.", this);
            return 0;
        }

        var available = new List<CollectibleTrash>();
        foreach (var prefab in _prefabs) if (prefab != null) available.Add(prefab);
        if (available.Count == 0) return 0;

        Physics.SyncTransforms();
        int missing = Mathf.Max(0, _itemCount - ActiveCount);
        int added = 0;
        for (int i = 0; i < missing; i++)
        {
            bool found = false;
            Vector3 position = Vector3.zero;
            for (int attempt = 0; attempt < _attemptsPerItem; attempt++)
            {
                Vector3 half = _areaSize * 0.5f;
                position = transform.TransformPoint(new Vector3(
                    Random.Range(-half.x, half.x),
                    Random.Range(-half.y, half.y),
                    Random.Range(-half.z, half.z)));
                if (Physics.CheckSphere(position, _clearanceRadius, _obstacleLayers, QueryTriggerInteraction.Ignore)) continue;

                bool tooClose = false;
                foreach (var item in _spawned)
                {
                    if (item != null && Vector3.Distance(position, item.transform.position) < _minimumSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) continue;
                found = true;
                break;
            }
            if (!found) continue;

            var selected = available[Random.Range(0, available.Count)];
            // Random yaw preserves the source object's upright orientation.
            Quaternion rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up) * selected.transform.rotation;
            var spawned = Instantiate(selected, position, rotation, transform);
            _spawned.Add(spawned);
            ItemSpawned?.Invoke(spawned);
            added++;
        }

        if (added < missing)
            Debug.LogWarning("Trash spawn volume is crowded; spawned " + added + " of " + missing + " requested items.", this);
        return added;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.8f, 0.8f, 0.8f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, _areaSize);
    }
}
