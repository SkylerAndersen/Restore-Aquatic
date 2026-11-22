using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class FixWaterBounds : MonoBehaviour
{
	public float extraHeight = -50f; 

	void Start()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;

		Bounds newBounds = mesh.bounds;
		newBounds.Expand(new Vector3(0, extraHeight, 0));

		mesh.bounds = newBounds;
	}
}