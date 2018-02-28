using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[ExecuteInEditMode]
	public abstract class DeformerBase : MonoBehaviour
	{
		public bool discardChangesOnDestroy = true;

		[SerializeField, HideInInspector]
		protected MeshFilter target;
		[SerializeField, HideInInspector]
		protected Chunk[] chunks;
		[SerializeField, HideInInspector]
		protected Mesh originalMesh;

		private List<Vector3> originalNormals = new List<Vector3> ();

		protected int deformChunkIndex;
		protected bool asyncUpdateInProgress { get; private set; }

		[SerializeField, HideInInspector]
		private int maxVerticesPerFrame = 500;
		public int MaxVerticesPerChunk
		{
			get { return maxVerticesPerFrame; }
			set { maxVerticesPerFrame = Mathf.Clamp (value, 100, VertexCount); }
		}
		public int ChunkCount { get { return Mathf.CeilToInt (VertexCount / MaxVerticesPerChunk); } }
		public int VertexCount { get { return originalMesh.vertexCount; } }
		public float SyncedTime { get; private set; }
		public float SyncedDeltaTime { get; private set; }

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

		public void ChangeMesh (Mesh mesh)
		{
			originalMesh = MeshUtil.Copy (mesh);
			target.sharedMesh = MeshUtil.Copy (mesh);
			target.sharedMesh.GetNormals (originalNormals);

			deformChunkIndex = 0;

			// Create chunk data.
			RecreateChunks ();
		}

		public void UpdateMeshInstant (NormalsCalculationMode normalsCalculation, float smoothingAngle, bool updateSyncedTime = false, bool recalculateBounds = true)
		{
			UpdateChunkTransformData ();

			// Reset chunks if deformation isn't finished or just starting
			if (deformChunkIndex != 0 && deformChunkIndex != ChunkCount - 1)
				ResetChunks ();

			if (updateSyncedTime)
				UpdateSyncedTime ();
			DeformChunks ();
			ApplyChunksToTarget (normalsCalculation, smoothingAngle, recalculateBounds);
			ResetChunks ();
			deformChunkIndex = 0;
		}

		public void UpdateMesh (UpdateMode updateMode, NormalsCalculationMode normalsCalculation, float smoothingAngle, bool recalculateBounds = true)
		{
			switch (updateMode)
			{
				case UpdateMode.Update:
					// If there's only one chunk, update all chunks and immediately apply
					// changes to the mesh.
					if (ChunkCount == 1)
						UpdateMeshInstant (normalsCalculation, smoothingAngle, true);
					// Otherwise deform the current chunk.
					else
					{
						// If the current chunk is the last chunk, apply the changes to the chunks.
						if (deformChunkIndex >= chunks.Length)
						{
							UpdateSyncedTime ();
							UpdateChunkTransformData ();
							ApplyChunksToTarget (normalsCalculation, smoothingAngle, recalculateBounds);
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
					ApplyChunksToTarget (NormalsCalculationMode.Original, smoothingAngle, recalculateBounds);
					return;
			}
		}

		public async void UpdateMeshAsync (NormalsCalculationMode normalsCalculation, float smoothingAngle, bool recalculateBounds = true)
		{
			if (asyncUpdateInProgress)
				return;
			UpdateChunkTransformData ();
			UpdateSyncedTime ();
			asyncUpdateInProgress = true;
			await new WaitForBackgroundThread ();
			DeformChunks ();
			await new WaitForUpdate ();
			asyncUpdateInProgress = false;
			ApplyChunksToTarget (normalsCalculation, smoothingAngle, recalculateBounds);
			ResetChunks ();
		}

		private void UpdateSyncedTime ()
		{
			SyncedDeltaTime = Time.time - SyncedTime;
			SyncedTime = Time.time;
		}

		public void RecreateChunks ()
		{
			chunks = ChunkUtil.CreateChunks (originalMesh, ChunkCount);
			UpdateChunkTransformData ();
		}

		public void UpdateChunkTransformData ()
		{
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
				chunks[chunkIndex].transformData = new TransformData (transform);
		}

		protected void ApplyChunksToTarget (NormalsCalculationMode normalsCalculation, float smoothingAngle, bool recalculateBounds)
		{
			ChunkUtil.ApplyChunks (chunks, target.sharedMesh);

			switch (normalsCalculation)
			{
				case NormalsCalculationMode.Unity:
					target.sharedMesh.RecalculateNormals ();
					break;
				case NormalsCalculationMode.Smooth:
					target.sharedMesh.RecalculateNormals (smoothingAngle);
					break;
				case NormalsCalculationMode.Maintain:
					break;
				case NormalsCalculationMode.Original:
					target.sharedMesh.SetNormals (originalNormals);
					break;
			}

			if (recalculateBounds)
				target.sharedMesh.RecalculateBounds ();
		}

		protected abstract void DeformChunk (int index, bool notifyPrePostModify = false);
		protected abstract void DeformChunks ();

		protected void ResetChunks ()
		{
			ChunkUtil.ResetChunks (chunks);
		}

		public void DiscardChanges ()
		{
			if (originalMesh != null && target != null)
				target.sharedMesh = MeshUtil.Copy (originalMesh);
		}
	}
}