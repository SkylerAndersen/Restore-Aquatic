using System.Collections.Generic;
using UnityEngine;

public class TrashScore : MonoBehaviour
{
    [Header("Collection Score")]
    [SerializeField] private RandomTrashSpawner _spawner;
    [SerializeField, Min(1)] private int _pointsPerItem = 10;
    [SerializeField] private TMPro.TMP_Text _scoreLabel;

    private readonly HashSet<CollectibleTrash> _trackedItems = new HashSet<CollectibleTrash>();

    public int Score { get; private set; }
    public int CollectedCount { get; private set; }

    private void OnEnable()
    {
        Connect();
    }

    public void Connect()
    {
        if (_spawner != null)
        {
            _spawner.ItemSpawned -= Track;
            _spawner.ItemSpawned += Track;
            foreach (var item in _spawner.GetActiveItems()) Track(item);
        }
        RefreshLabel();
    }

    private void OnDisable()
    {
        if (_spawner != null) _spawner.ItemSpawned -= Track;
        foreach (var item in _trackedItems) if (item != null) item.Collected -= HandleCollected;
        _trackedItems.Clear();
    }

    private void Track(CollectibleTrash item)
    {
        if (item == null || item.IsCollected || !_trackedItems.Add(item)) return;
        item.Collected += HandleCollected;
    }

    private void HandleCollected(CollectibleTrash item)
    {
        if (!_trackedItems.Remove(item)) return;
        item.Collected -= HandleCollected;
        Score += _pointsPerItem;
        CollectedCount++;
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (_scoreLabel != null)
            _scoreLabel.SetText("SCORE  {0}\nTRASH COLLECTED  {1}", (float)Score, (float)CollectedCount);
    }
}
