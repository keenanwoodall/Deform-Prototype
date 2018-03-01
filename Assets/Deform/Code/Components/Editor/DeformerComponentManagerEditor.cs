﻿using UnityEngine;
using UnityEditor;

namespace Deform
{
	[CustomEditor (typeof (DeformerComponentManager))]
	public class DeformerComponentManagerEditor : Editor
	{
		private const string TINY_INDENT = @"  ";
		private const string SMALL_INDENT = @"    ";
		private const string LARGE_INDENT = @"      ";

		[SerializeField]
		private bool showDebug = false;

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
			normalsCalculation = (NormalsCalculationMode)EditorGUILayout.EnumPopup (new GUIContent ("Normals Calculation", "Unity - Pretty Fast. Uses Unity's runtime normal recalculation\nSmooth - Very Slow. Looks much better than Unity's method.\nMaintain - Fastest. Keeps the current normals.\nOriginal - Very Fast. Applies the normals of the original, unmodified mesh."), normalsCalculation);
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
			EditorGUILayout.BeginHorizontal (GUILayout.MaxWidth (5));
			EditorGUILayout.LabelField ("Debug Info", GUILayout.MaxWidth (70));
			showDebug = GUILayout.Toggle (showDebug, string.Empty);
			EditorGUILayout.EndHorizontal ();
			if (!showDebug)
				return;

			EditorGUILayout.LabelField (string.Format ("{0}Vertex Count: {1}", TINY_INDENT, manager.VertexCount));
			EditorGUILayout.LabelField (string.Format ("{0}Chunk Count: {1}", TINY_INDENT, manager.ChunkCount));
			//EditorGUILayout.LabelField (string.Format ("Time: {0}", manager.SyncedTime));
			//EditorGUILayout.LabelField (string.Format ("Delta Time: {0}", manager.SyncedDeltaTime));
			EditorGUILayout.LabelField (TINY_INDENT + "Deformers:");
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