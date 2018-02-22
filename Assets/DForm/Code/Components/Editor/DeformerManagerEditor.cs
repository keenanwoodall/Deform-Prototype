using UnityEngine;
using UnityEditor;

namespace Deform
{
	[CustomEditor (typeof (DeformerManager))]
	public class DeformerManagerEditor : Editor
	{
		private const string SMALL_INDENT = @"    ";
		private const string LARGE_INDENT = @"      ";

		public override void OnInspectorGUI ()
		{
			var manager = target as DeformerManager;

			base.OnInspectorGUI ();

			foreach (var deformer in manager.GetDeformers ())
			{
				var indent = deformer.update ? LARGE_INDENT : SMALL_INDENT;
				var style = deformer.update ? EditorStyles.whiteMiniLabel : EditorStyles.miniBoldLabel;
				var label = new GUIContent (indent + deformer.GetType ().Name);
				EditorGUILayout.LabelField (label, style);
			}

			if (!Application.isPlaying)
				manager.UpdateMesh ();
				
			Repaint ();
		}
	}
}