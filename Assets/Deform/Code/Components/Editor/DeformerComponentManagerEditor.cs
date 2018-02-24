using UnityEngine;
using UnityEditor;

namespace Deform
{
	[CustomEditor (typeof (DeformerComponentManager))]
	public class DeformerComponentManagerEditor : Editor
	{
		private const string SMALL_INDENT = @"    ";
		private const string LARGE_INDENT = @"      ";
		private const bool SHOW_DEBUG_INFO = true;


		public override void OnInspectorGUI ()
		{
			var manager = target as DeformerComponentManager;

			base.OnInspectorGUI ();

			EditorGUI.BeginChangeCheck ();
			var label = new GUIContent ("Max Vertices Per Frame");
			var maxVerticesPerFrame = EditorGUILayout.IntSlider (label, manager.MaxVerticesPerFrame, 50, manager.VertexCount);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Changed Max Vertices Per Frame");
				manager.MaxVerticesPerFrame = maxVerticesPerFrame;
				manager.RecreateChunks ();
			}

			if (SHOW_DEBUG_INFO)
			{
				EditorGUILayout.LabelField (string.Format ("Vertex Count: {0}", manager.VertexCount));
				EditorGUILayout.LabelField (string.Format ("Chunk Count: {0}", manager.ChunkCount));

				var deformers = manager.GetDeformers ();
				foreach (var deformer in deformers)
				{
					EditorGUILayout.LabelField (deformer.GetType ().Name);
				}
			}

			manager.RefreshDeformerOrder ();

			if (!Application.isPlaying)
				manager.UpdateMesh ();

			Repaint ();
		}
	}
}