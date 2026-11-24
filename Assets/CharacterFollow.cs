using UnityEngine;

public class CharacterFollow : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float x;
    [SerializeField] public float y;
    [SerializeField] public float z;
    [SerializeField] public float first;
    [SerializeField] public float second;
    [SerializeField] public float third;
    [SerializeField] public float follow_speed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = UnityEngine.Quaternion.Euler(first, second, third);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null) return;

        transform.rotation = UnityEngine.Quaternion.Euler(first, second, third);
        UnityEngine.Vector3 target_pos = new UnityEngine.Vector3(player.position.x + x, player.position.y + y, player.position.z + z);
        float blend = 1f - Mathf.Exp(-follow_speed * Time.deltaTime);
        transform.position = UnityEngine.Vector3.Lerp(transform.position, target_pos, blend);
    }
}
