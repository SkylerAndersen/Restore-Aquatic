using UnityEngine;

public class BasicPlayerController : MonoBehaviour
{
    [Header("Swimming")]
    [SerializeField, Min(0f)] private float look_speed = 10f;
    [SerializeField, Min(0f), Tooltip("Maximum swim speed in world units per second.")] private float speed = 4f;
    [SerializeField, Min(0f)] private float acceleration = 8f;
    [SerializeField, Min(0f)] private float deceleration = 10f;

    private Vector2 desired_direction;
    private float desired_vertical;
    private Rigidbody rb;
    private Quaternion initialRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        initialRotation = rb.rotation;
    }

    void OnDisable()
    {
        desired_direction = Vector2.zero;
        desired_vertical = 0f;
    }

    void FixedUpdate()
    {
        Vector3 direction = Vector3.ClampMagnitude(
            new Vector3(desired_direction.x, desired_vertical, desired_direction.y), 1f);
        Vector3 targetVelocity = direction * speed;
        float rate = direction.sqrMagnitude > 0f ? acceleration : deceleration;
        Vector3 nextVelocity = Vector3.MoveTowards(rb.linearVelocity, targetVelocity, rate * Time.fixedDeltaTime);
        rb.AddForce(nextVelocity - rb.linearVelocity, ForceMode.VelocityChange);

        if (desired_direction.sqrMagnitude > 0.0001f)
        {
            float yaw = Mathf.Atan2(desired_direction.x, desired_direction.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(yaw, Vector3.up) * initialRotation;
            float blend = 1f - Mathf.Exp(-look_speed * Time.fixedDeltaTime);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, blend));
        }
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        desired_direction = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
    }

    public void OnVertical(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        desired_vertical = context.canceled ? 0f : context.ReadValue<float>();
    }
}

