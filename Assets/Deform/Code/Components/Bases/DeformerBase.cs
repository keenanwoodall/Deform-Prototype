using System;
using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	[ExecuteInEditMode]
	public abstract class DeformerBase : MonoBehaviour
	{
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
		private int maxVerticesPerFrame = 2000;
		public int MaxVerticesPerChunk
		{
			get { return maxVerticesPerFrame; }
			set { maxVerticesPerFrame = Mathf.Clamp (value, 1, VertexCount); }
		}
		public int FrameSplitChunkCount { get { return Mathf.CeilToInt (VertexCount / MaxVerticesPerChunk); } }
		public int ChunkCount { get { return chunks.Length; } }
		public int VertexCount { get { return originalMesh.vertexCount; } }
		public float SyncedTime { get; private set; }
		public float SyncedDeltaTime { get; private set; }
		public TransformData SyncedTransform { get; private set; }
		public Bounds Bounds { get; private set; }
		public MeshFilter Target { get { return target; } }

		private void OnDestroy ()
		{
			DiscardChanges ();
		}

		public void ChangeTarget (MeshFilter meshFilter, bool createChunks = true)
		{
			// Assign the target.
			target = meshFilter;

			// If it's not null, the object was probably duplicated
			if (originalMesh == null)
				// Store the original mesh.
				originalMesh = MeshUtil.Copy (target.sharedMesh);
			// Change the mesh to one we can modify.
			target.sharedMesh = MeshUtil.Copy (originalMesh);
			Bounds = target.sharedMesh.bounds;
			// Cache the original normals.
			target.sharedMesh.GetNormals (originalNormals);

			deformChunkIndex = 0;

			// Create chunk data.
			if (createChunks)
				RecreateChunks ();
		}

		public void ChangeMesh (Mesh mesh)
		{
			originalMesh = MeshUtil.Copy (mesh);
			target.sharedMesh = MeshUtil.Copy (mesh);
			Bounds = target.sharedMesh.bounds;
			target.sharedMesh.GetNormals (originalNormals);

			deformChunkIndex = 0;

			// Create chunk data.
			RecreateChunks ();
		}

		public void UpdateMeshInstant (NormalsCalculationMode normalsCalculation, float smoothingAngle)
		{
			// Reset chunks if deformation isn't finished or just starting
			if (deformChunkIndex != 0 && deformChunkIndex != FrameSplitChunkCount - 1)
				ResetChunks ();

			DeformChunks ();
			ApplyChunksToTarget (normalsCalculation, smoothingAngle);
			ResetChunks ();
			deformChunkIndex = 0;
		}

		public async void UpdateMeshAsync (NormalsCalculationMode normalsCalculation, float smoothingAngle, Action onComplete = null)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				Debug.LogError ("UpdateMeshAsync doesn't work in edit-mode");
				return;
			}
#endif
			if (asyncUpdateInProgress)
				return;

			asyncUpdateInProgress = true;
			await new WaitForBackgroundThread ();
			DeformChunks ();
			await new WaitForUpdate ();
			asyncUpdateInProgress = false;
			ApplyChunksToTarget (normalsCalculation, smoothingAngle);
			ResetChunks ();

			if (onComplete != null)
				onComplete.Invoke ();
		}

		public void UpdateNormals (NormalsCalculationMode normalsCalculation, float smoothingAngle)
		{
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
		}

		public void UpdateSyncedTime ()
		{
			SyncedDeltaTime = Time.time - SyncedTime;
			SyncedTime = Time.time;
		}

		public void UpdateTransformData ()
		{
			SyncedTransform = new TransformData (transform);
		}

		public void RecreateChunks (bool forceSingleChunk = false)
		{
			chunks = ChunkUtil.CreateChunks (originalMesh, forceSingleChunk ? 1 : FrameSplitChunkCount);
		}

		protected void ApplyChunksToTarget (NormalsCalculationMode normalsCalculation, float smoothingAngle)
		{
			ChunkUtil.ApplyChunks (chunks, target.sharedMesh);
			UpdateNormals (normalsCalculation, smoothingAngle);

			target.sharedMesh.RecalculateBounds ();
		}

		protected abstract void DeformChunk (int index);
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