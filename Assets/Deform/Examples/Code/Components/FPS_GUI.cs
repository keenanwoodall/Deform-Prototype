using UnityEngine;

public class FPS_GUI : MonoBehaviour
{
	public int updateRate = 4;
	private int frameCount = 0;
	private float deltaTime = 0f;
	private float fps = 0f;

	private void Update ()
	{
		frameCount++;
		deltaTime += Time.deltaTime;
		if (deltaTime > 1f / updateRate)
		{
			fps = frameCount / deltaTime;
			frameCount = 0;
			deltaTime -= 1f / updateRate;
		}
	}

	private void OnGUI ()
	{
		GUI.Label (new Rect (10, 10, 100, 20), fps.ToString ("n0"));
	}
}