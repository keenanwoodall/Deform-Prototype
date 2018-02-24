using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[ExecuteInEditMode]
	public abstract class DeformerManagerBase : MonoBehaviour
	{
		public enum UpdateMode { Update, Pause, Stop }
		public UpdateMode updateMode = UpdateMode.Update;
		public NormalsCalculation normalsCalculation = NormalsCalculation.Unity;
		public bool recalculateBounds = true;
		public bool multiFrameCalculation = true;
		public bool discardChangesOnDestroy = true;

		[SerializeField, HideInInspector]
		protected MeshFilter target;
		[SerializeField, HideInInspector]
		protected Chunk[] chunks;
		[SerializeField, HideInInspector]
		protected Mesh originalMesh;

		private bool usingOriginalNormals;
		private List<Vector3> originalNormals = new List<Vector3> ();

		protected int deformChunkIndex;

		[SerializeField, HideInInspector]
		private int maxVerticesPerFrame = 500;
		public int MaxVerticesPerFrame
		{
			get { return maxVerticesPerFrame; }
			set { maxVerticesPerFrame = Mathf.Clamp (value, 50, VertexCount); }
		}
		public int ChunkCount { get { return Mathf.CeilToInt (VertexCount / MaxVerticesPerFrame); } }
		public int VertexCount { get { return originalMesh.vertexCount; } }
		public float SyncedTime { get; private set; }
		public float SyncedDeltaTime { get; private set; }

		private void Awake ()
		{
			DiscardChanges ();
			ChangeTarget (GetComponent<MeshFilter> ());
#if UNITY_EDITOR
			if (!Application.isPlaying || (Application.isPlaying && Time.frameCount == 0))
				UpdateMeshInstant ();
#else
				UpdateMeshInstant ();
#endif
		}
		private void OnDestroy ()
		{
			if (discardChangesOnDestroy)
				DiscardChanges ();
		}

		public void ChangeTarget (MeshFilter meshFilter)
		{
			// Assign the target.
			target = meshFilter;
			// If it's not null, the object was probably duplicated
			if (originalMesh == null)
				// Store the original mesh.
				originalMesh = MeshUtil.Copy (target.sharedMesh);
			// Change the mesh to one we can modify.
			target.sharedMesh = MeshUtil.Copy (originalMesh);
			// Cache the original normals.
			target.sharedMesh.GetNormals (originalNormals);

			deformChunkIndex = 0;

			// Create chunk data.
			RecreateChunks ();
		}

		private void UpdateSyncedTime ()
		{
			SyncedDeltaTime = Time.time - SyncedTime;
			SyncedTime = Time.time;
		}

		public void RecreateChunks ()
		{
			chunks = ChunkUtil.CreateChunks (originalMesh, ChunkCount);
		}

		protected void ApplyChunksToTarget (NormalsCalculation normalsCalculation, bool recalculateBounds)
		{
			ChunkUtil.ApplyChunks (chunks, target.sharedMesh);

			switch (normalsCalculation)
			{
				case NormalsCalculation.Unity:
					target.sharedMesh.RecalculateNormals ();
					break;
				case NormalsCalculation.Smooth:
					target.sharedMesh.RecalculateNormals (60f);
					break;
				case NormalsCalculation.Maintain:
					break;
				case NormalsCalculation.Original:
					target.sharedMesh.SetNormals (originalNormals);
					break;
			}

			if (recalculateBounds)
				target.sharedMesh.RecalculateBounds ();
		}

		protected void ResetChunks ()
		{
			ChunkUtil.ResetChunks (chunks);
		}

		public void UpdateMeshInstant ()
		{
			DeformChunks ();
			ApplyChunksToTarget (normalsCalculation, recalculateBounds);
			ResetChunks ();
			deformChunkIndex = 0;
		}

		public void UpdateMesh ()
		{
			switch (updateMode)
			{
				case UpdateMode.Update:
					// If there's only one chunk, update all chunks and immediatly apply
					// changes to the mesh.
					if (ChunkCount == 1 || !multiFrameCalculation)
					{
						UpdateSyncedTime ();
						UpdateMeshInstant ();
					}
					// Otherwise deform the current chunk.
					else
					{
						// If the current chunk is the last chunk, apply the changes to the chunks.
						if (deformChunkIndex >= chunks.Length)
						{
							UpdateSyncedTime ();
							ApplyChunksToTarget (normalsCalculation, recalculateBounds);
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
					ApplyChunksToTarget (NormalsCalculation.Original, recalculateBounds);
					return;
			}
		}

		protected abstract void DeformChunk (int index, bool notifyPrePostModify = false);
		protected abstract void DeformChunks ();

		public void DiscardChanges ()
		{
			if (originalMesh != null && target != null)
				target.sharedMesh = MeshUtil.Copy (originalMesh);
		}
	}
}