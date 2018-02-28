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

			DrawThreadedGUI (manager);
			if (!manager.threaded || !Application.isPlaying)
			{
				DrawUpdateModeGUI (manager);
				DrawMaxVerticesPerFrameGUI (manager);
			}
			DrawNormalsCalculationGUI (manager);
			DrawRecalculateBoundsGUI (manager);
			DrawMultiframeCalculationGUI (manager);
			DrawDiscardChangesOnDestroyGUI (manager);

			//DrawDebugGUI (manager);

			manager.RefreshDeformerOrder ();

			if (!Application.isPlaying)
				manager.UpdateMesh (manager.updateMode, manager.normalsCalculation);

			Repaint ();
		}

		private void DrawThreadedGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var threaded = manager.threaded;
			threaded = EditorGUILayout.Toggle ("Threaded" + ((Application.isPlaying) ? "" : " (in play-mode)"), threaded);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Threaded");
				manager.threaded = threaded;
			}
		}

		private void DrawUpdateModeGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			GUILayout.BeginHorizontal ();
			var updateMode = manager.updateMode;
			if (updateMode != UpdateMode.Update)
				if (GUILayout.Button ("Play", GUILayout.Width (50)))
					updateMode = UpdateMode.Update;
			if (updateMode != UpdateMode.Pause && updateMode != UpdateMode.Stop)
				if (GUILayout.Button ("Pause", GUILayout.Width (50)))
					updateMode = UpdateMode.Pause;
			if (updateMode != UpdateMode.Stop)
				if (GUILayout.Button ("Stop", GUILayout.Width (50)))
					updateMode = UpdateMode.Stop;
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Update Mode");
				manager.updateMode = updateMode;
			}

			if (manager.updateMode == UpdateMode.Pause)
				if (GUILayout.Button ("Step", GUILayout.Width (50)))
					manager.UpdateMeshInstant (manager.normalsCalculation);
			GUILayout.EndHorizontal ();
		}

		private void DrawMaxVerticesPerFrameGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var label = new GUIContent ("Max Vertices Per Frame");
			var maxVerticesPerFrame = EditorGUILayout.DelayedIntField (label, manager.MaxVerticesPerFrame);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, " Max Vertices Per Frame");
				manager.MaxVerticesPerFrame = maxVerticesPerFrame;
				manager.RecreateChunks ();
			}
		}

		private void DrawNormalsCalculationGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var normalsCalculation = manager.normalsCalculation;
			normalsCalculation = (NormalsCalculation)EditorGUILayout.EnumPopup ("Normals Calculation", normalsCalculation);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Normals Calculation");
				manager.normalsCalculation = normalsCalculation;
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

		private void DrawMultiframeCalculationGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var multiFrameCalculation = manager.multiFrameCalculation;
			multiFrameCalculation = EditorGUILayout.Toggle ("Multiframe Calculation", multiFrameCalculation);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Multiframe Calculation");
				manager.multiFrameCalculation = multiFrameCalculation;
			}
		}

		private void DrawDiscardChangesOnDestroyGUI (DeformerComponentManager manager)
		{
			EditorGUI.BeginChangeCheck ();
			var discardChangesOnDestroy = manager.discardChangesOnDestroy;
			discardChangesOnDestroy = EditorGUILayout.Toggle ("Discard Changes On Destroy", discardChangesOnDestroy);
			if (EditorGUI.EndChangeCheck ())
			{
				Undo.RecordObject (manager, "Discard Changes On Destroy");
				manager.discardChangesOnDestroy = discardChangesOnDestroy;
			}
		}

		private void DrawDebugGUI (DeformerComponentManager manager)
		{
			if (!SHOW_DEBUG_INFO)
				return;

			EditorGUILayout.Separator ();

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