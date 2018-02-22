using UnityEngine;
using UnityEditor;

namespace Deform
{
	[CustomEditor (typeof (DeformerManager))]
	public class DeformerManagerEditor : Editor
	{
		public override void OnInspectorGUI ()
		{
			base.OnInspectorGUI ();

			if (Application.isPlaying)
				return;
			(target as DeformerManager).UpdateMesh ();
			Repaint ();
		}
	}
}