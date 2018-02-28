using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DeformerComponentManager : DeformerBase
	{
		public bool multithreaded = true;
		public UpdateMode updateMode = UpdateMode.Update;
		public NormalsCalculation normalsCalculation = NormalsCalculation.Unity;

		public bool recalculateBounds = true;

		[SerializeField, HideInInspector]
		private List<DeformerComponent> deformers = new List<DeformerComponent> ();

		private void Awake ()
		{
			DiscardChanges ();
			ChangeTarget (GetComponent<MeshFilter> ());
#if UNITY_EDITOR
			if (!Application.isPlaying || (Application.isPlaying && Time.frameCount == 0))
				UpdateMeshInstant (normalsCalculation);
#else
				UpdateMeshInstant (normalsCalculation);
#endif
		}

		public void Update ()
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				if (multithreaded)
					UpdateMeshAsync (normalsCalculation);
				else
					UpdateMesh (updateMode, normalsCalculation);
			}
#else
			if (threaded)
					UpdateMeshAsync (normalsCalculation);
				else
					UpdateMesh (updateMode, normalsCalculation);
			}
#endif
		}
		private void OnDestroy ()
		{
			deformers.Clear ();
		}

		private void NotifyPreModify ()
		{
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
			{
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PreModify ();
			}
		}
		private void NotifyPostModify ()
		{
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PostModify ();
		}

		protected override void DeformChunks ()
		{
			NotifyPreModify ();

			// Modify chunks
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
						chunks[chunkIndex] = deformers[deformerIndex].Modify (chunks[chunkIndex]);

			NotifyPostModify ();
		}

		protected override void DeformChunk (int index, bool notifyPrePostModify = false)
		{
			if (ChunkCount != chunks.Length)
				RecreateChunks ();

			if (notifyPrePostModify)
				NotifyPreModify ();

			// Modify chunk
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					chunks[index] = deformers[deformerIndex].Modify (chunks[index]);

			if (notifyPrePostModify)
				NotifyPostModify ();
		}

		public void AddDeformer (DeformerComponent deformer)
		{
			if (deformers == null)
				deformers = new List<DeformerComponent> ();
			if (!deformers.Contains (deformer))
				deformers.Add (deformer);
		}

		public void RemoveDeformer (DeformerComponent deformer)
		{
			if (deformers == null)
				return;
			if (deformers.Contains (deformer))
				deformers.Remove (deformer);
		}

		public void RefreshDeformerOrder ()
		{
			var oldDeformers = new List<DeformerComponent> ();
			oldDeformers.AddRange (deformers);
			deformers.Clear ();

			var currentDeformers = GetComponents<DeformerComponent> ();

			for (int i = 0; i < currentDeformers.Length; i++)
				if (oldDeformers.Contains (currentDeformers[i]))
					deformers.Add (currentDeformers[i]);
		}

		public List<DeformerComponent> GetDeformers ()
		{
			return deformers;
		}
	}
}