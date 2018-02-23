using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[RequireComponent (typeof (MeshFilter))]
	[ExecuteInEditMode]
	public class DeformerManager : DeformerManagerBase
	{
		public enum UpdateMode { Update, Pause, Stop }
		public UpdateMode updateMode = UpdateMode.Update;

		[SerializeField, HideInInspector]
		private List<DeformerComponent> deformers = new List<DeformerComponent> ();

		private int deformChunkIndex;

		public float SyncedTime { get; private set; }
		public float SyncedDeltaTime { get; private set; }

		private void Awake ()
		{
			SyncedTime = 0f;
			target = GetComponent<MeshFilter> ();

			DiscardChanges ();
			ChangeTarget (target);
			UpdateMesh ();
		}

		private void Update ()
		{
			UpdateMesh ();
		}

		public void UpdateMesh ()
		{
			switch (updateMode)
			{
				case UpdateMode.Update:
					// If there's only one chunk, update all chunks and immediatley apply
					// changes to the mesh.
					if (chunkCount == 1)
					{
						UpdateSyncedTime ();
						DeformChunks ();
						ApplyChunksToTarget ();
						ResetChunks ();
					}
					// Otherwise deform the current chunk.
					else
					{
						// If the current chunk is the last chunk, apply the changes to the chunks.
						if (deformChunkIndex >= chunks.Length)
						{
							UpdateSyncedTime ();
							ApplyChunksToTarget ();
							ResetChunks ();
							deformChunkIndex = 0;
						}
						DeformChunk (deformChunkIndex, deformChunkIndex == 0);
						deformChunkIndex++;
					}
					return;
				case UpdateMode.Pause:
					return;
				case UpdateMode.Stop:
					ResetChunks ();
					ApplyChunksToTarget ();
					return;
			}
		}

		private void UpdateSyncedTime ()
		{
			SyncedDeltaTime = Time.smoothDeltaTime * chunkCount;
			SyncedTime += SyncedDeltaTime;
		}

		private void NotifyPreModify ()
		{
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PreModify ();
		}
		private void NotifyPostModify ()
		{
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					deformers[deformerIndex].PostModify ();
		}

		private void DeformChunks ()
		{
			NotifyPreModify ();

			// Modify chunks
			for (var deformerIndex = 0; deformerIndex < deformers.Count; deformerIndex++)
				if (deformers[deformerIndex].update)
					for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
						chunks[chunkIndex].vertexData = deformers[deformerIndex].Modify (chunks[chunkIndex].vertexData);

			NotifyPostModify ();
		}

		private void DeformChunk (int index, bool notifyPrePostModify = false)
		{
			if (chunkCount != chunks.Length)
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