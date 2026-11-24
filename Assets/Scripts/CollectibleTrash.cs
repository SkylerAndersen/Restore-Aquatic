using UnityEngine;

public class CollectibleTrash : MonoBehaviour
{
    [Header("Collection")]
    [SerializeField] private UnityEngine.Events.UnityEvent _onCollected = new UnityEngine.Events.UnityEvent();

    private bool _isCollected;

    public bool IsCollected => _isCollected;
    public event System.Action<CollectibleTrash> Collected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody == null) return;
        if (other.attachedRigidbody.TryGetComponent<BasicPlayerController>(out _)) Collect();
    }

    public bool Collect()
    {
        if (_isCollected) return false;
        _isCollected = true;
        gameObject.SetActive(false);
        Collected?.Invoke(this);
        _onCollected.Invoke();
        if (Application.isPlaying) Destroy(gameObject);
        return true;
    }
}
