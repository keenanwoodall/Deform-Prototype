using UnityEngine;
using UnityEditor;

namespace Deform
{
	[CustomEditor (typeof (DeformerManager))]
	public class DeformerManagerEditor : Editor
	{
		private const string SMALL_INDENT = @"    ";
		private const string LARGE_INDENT = @"      ";
		private const bool SHOW_DEFORMERS = false;

		public override void OnInspectorGUI ()
		{
			var manager = target as DeformerManager;

			base.OnInspectorGUI ();

			if (GUI.changed)
			{
				// If the gui has changed, the chunk count may have been modified.
				manager.RecreateChunks ();
			}

			if (SHOW_DEFORMERS)
			{
				foreach (var deformer in manager.GetDeformers ())
				{
					var indent = deformer.update ? LARGE_INDENT : SMALL_INDENT;
					var style = deformer.update ? EditorStyles.whiteMiniLabel : EditorStyles.miniBoldLabel;
					var label = new GUIContent (indent + deformer.GetType ().Name);
					EditorGUILayout.LabelField (label, style);
				}
			}

			Repaint ();
		}
	}
}