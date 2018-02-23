using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DeformerManager : DeformerManagerBase
	{
		[SerializeField, HideInInspector]
		private List<DeformerComponent> deformers = new List<DeformerComponent> ();

		private void Update ()
		{
#if UNITY_EDITOR
			if (Application.isPlaying)
				UpdateMesh ();
#else
			UpdateMesh ();
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
						chunks[chunkIndex].vertexData = deformers[deformerIndex].Modify (chunks[chunkIndex].vertexData);

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
					chunks[index].vertexData = deformers[deformerIndex].Modify (chunks[index].vertexData);

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

		public List<DeformerComponent> GetDeformers ()
		{
			return deformers;
		}
	}
}