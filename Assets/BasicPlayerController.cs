using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class BasicPlayerController : MonoBehaviour
{
    [SerializeField] public float look_speed;
    [SerializeField] public float speed;
    private UnityEngine.Vector2 desired_direction;
    private float desired_vertical;
    private Rigidbody rb;
    private float vertical;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        desired_direction = new UnityEngine.Vector2(0f, 0f);
        desired_vertical = 0f;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float desired_look_angle = (desired_direction.x, desired_direction.y) switch
        {
            (0,0) => transform.eulerAngles.y,
            (1,0) => 90f,
            (1,1) => 45f,
            (0,1) => 0f,
            (-1,1) => -45f,
            (-1,0) => -90f,
            (-1,-1) => -135f,
            (0,-1) => 180f,
            (1,-1) => 135f,
            _ => transform.eulerAngles.y
        };

        float new_rot = Mathf.LerpAngle(transform.eulerAngles.y, desired_look_angle, look_speed * Time.deltaTime);
        transform.rotation = UnityEngine.Quaternion.Euler(transform.eulerAngles.x, new_rot, transform.eulerAngles.z);
        UnityEngine.Vector3 force = new UnityEngine.Vector3(desired_direction.x * speed, desired_vertical * speed, desired_direction.y * speed);
        rb.AddForce(force, ForceMode.Force);
    }

    public void OnMove (InputAction.CallbackContext context)
    {
        desired_direction = context.ReadValue<UnityEngine.Vector2>();
        desired_direction = new UnityEngine.Vector2(Mathf.Round(desired_direction.x), Mathf.Round(desired_direction.y));
    }

    public void OnVertical (InputAction.CallbackContext context)
    {
        desired_vertical = context.ReadValue<float>();
    }
}
