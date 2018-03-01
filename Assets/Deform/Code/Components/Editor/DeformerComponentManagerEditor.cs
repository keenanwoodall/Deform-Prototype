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

			DrawUpdateModeGUI (manager);
			if (manager.updateMode == UpdateMode.UpdateFrameSplit)
				DrawMaxVerticesPerChunkGUI (manager);
			DrawNormalsCalculationGUI (manager);
			if (manager.normalsCalculation == NormalsCalculationMode.Smooth)
				DrawSmoothAngleGUI (manager);
			DrawRecalculateBoundsGUI (manager);

			DrawDebugGUI (manager);

			manager.RefreshDeformerOrder ();

			if (!Application.isPlaying)
				manager.Update ();

			Repaint ();
		}

		private void DrawUpdateModeGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var updateMode = manager.updateMode;
			updateMode = (UpdateMode)EditorGUILayout.EnumPopup ("Update Mode", updateMode);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Update Mode");
				manager.updateMode = updateMode;
				if (updateMode == UpdateMode.UpdateInstant || updateMode == UpdateMode.UpdateAsync)
					manager.RecreateChunks (true);
				else if (updateMode == UpdateMode.UpdateFrameSplit)
					manager.RecreateChunks ();
			}

			if (updateMode == UpdateMode.UpdateAsync)
			{
				EditorGUILayout.HelpBox ("UpdateAsync only works in Play-Mode, UpdateInstant will be used in the editor", MessageType.Info);
			}
		}

		private void DrawMaxVerticesPerChunkGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var label = new GUIContent ("Max Vertices Per Chunk");
			var maxVerticesPerChunk = EditorGUILayout.DelayedIntField (label, manager.MaxVerticesPerChunk);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, " Max Vertices Per Chunk");
				manager.MaxVerticesPerChunk = maxVerticesPerChunk;
				if (manager.updateMode == UpdateMode.UpdateFrameSplit)
					manager.RecreateChunks ();
			}
		}

		private void DrawNormalsCalculationGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var normalsCalculation = manager.normalsCalculation;
			normalsCalculation = (NormalsCalculationMode)EditorGUILayout.EnumPopup ("Normals Calculation", normalsCalculation);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Normals Calculation");
				manager.normalsCalculation = normalsCalculation;
			}
		}

		private void DrawSmoothAngleGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var smoothingAngle = manager.SmoothingAngle;
			smoothingAngle = EditorGUILayout.FloatField ("Smoothing Angle", smoothingAngle);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Smoothing Angle");
				manager.SmoothingAngle = smoothingAngle;
			}
		}

		private void DrawRecalculateBoundsGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var recalculateBounds = manager.recalculateBounds;
			recalculateBounds = EditorGUILayout.Toggle ("Recalculate Bounds", recalculateBounds);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Recalculate Bounds");
				manager.recalculateBounds = recalculateBounds;
			}
		}

		private void DrawDebugGUI (DeformerComponentManager manager)
		{
			if (!SHOW_DEBUG_INFO)
				return;

			EditorGUILayout.Separator ();


			EditorGUILayout.LabelField ("Debug:");
			EditorGUILayout.LabelField (string.Format ("Vertex Count: {0}", manager.VertexCount));
			EditorGUILayout.LabelField (string.Format ("Chunk Count: {0}", manager.ChunkCount));
			EditorGUILayout.LabelField (string.Format ("Time: {0}", manager.SyncedTime));
			EditorGUILayout.LabelField (string.Format ("Delta Time: {0}", manager.SyncedDeltaTime));

			var deformers = manager.GetDeformers ();
			foreach (var deformer in deformers)
			{
				var indent = deformer.update ? LARGE_INDENT : SMALL_INDENT;
				EditorGUI.BeginDisabledGroup (!deformer.update);
				EditorGUILayout.LabelField (indent + deformer.GetType ().Name);
				EditorGUI.EndDisabledGroup ();
			}
		}
	}
}