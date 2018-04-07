using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
	public Vector3 amount;
	public Space space;

	private void Update ()
	{
		transform.Rotate (amount * Time.deltaTime, space);
	}
}