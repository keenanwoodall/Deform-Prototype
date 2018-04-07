using UnityEngine;

public class SinScale : MonoBehaviour
{
	public Vector3 a = Vector3.one, b = Vector3.one;
	public float speed = 1f;

	private void Update ()
	{
		var t = (Mathf.Sin (Time.time * speed) + 1f) / 2f;
		transform.localScale = Vector3.Lerp (a, b, t);
	}
}