using UnityEngine;

public struct TransformData
{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 localScale;

	public TransformData (Vector3 position, Quaternion rotation, Vector3 localScale)
	{
		this.position = position;
		this.rotation = rotation;
		this.localScale = localScale;
	}

	public TransformData (Transform transform)
	{
		this.position = transform.position;
		this.rotation = transform.rotation;
		this.localScale = transform.localScale;
	}
}