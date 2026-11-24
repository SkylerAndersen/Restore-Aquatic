using UnityEngine;

public class WaveSimulator : MonoBehaviour
{
    private Rigidbody rb;
    private bool movingUp;
    [SerializeField] public float maxHeight;
    [SerializeField] public float minHeight;
    [SerializeField] public float speed;
    private UnityEngine.Vector3 topTarget;
    private UnityEngine.Vector3 bottomTarget;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movingUp = false;
        topTarget = new UnityEngine.Vector3(0f, maxHeight + 2f, 0f);
        bottomTarget = new UnityEngine.Vector3(0f, minHeight - 2f, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float height = transform.position.y;
        if (movingUp && height < maxHeight)
        {
            UnityEngine.Vector3 progress = UnityEngine.Vector3.Slerp(rb.position, topTarget, speed * Time.fixedDeltaTime);
            rb.MovePosition(progress);
        }
        if (!movingUp && height > minHeight)
        {
            UnityEngine.Vector3 progress = UnityEngine.Vector3.Slerp(rb.position, bottomTarget, speed * 0.5f * Time.fixedDeltaTime);
            rb.MovePosition(progress);
        }

        if (movingUp && height > maxHeight)
        {
            movingUp = false;
        }

        if (!movingUp && height < minHeight)
        {
            movingUp = true;
        }
    }
}
