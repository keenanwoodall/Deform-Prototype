using UnityEngine;

public struct TransformData
{
	public Vector3 position;
	public Vector3 localPosition;
	public Quaternion rotation;
	public Quaternion localRotation;
	public Vector3 localScale;
	public Vector3 lossyScale;
	public Vector3 right;
	public Vector3 up;
	public Vector3 forward;

	public TransformData (Transform transform)
	{
		position = transform.position;
		localPosition = transform.localPosition;
		rotation = transform.rotation;
		localRotation = transform.localRotation;
		localScale = transform.localScale;
		lossyScale = transform.lossyScale;
		right = transform.right;
		up = transform.up;
		forward = transform.forward;
	}
}