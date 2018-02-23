using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	public abstract class DeformerManagerBase : MonoBehaviour
	{
		public int chunkCount = 1;
		public bool discardChangesOnDestroy = true;
		public bool recalculateBounds = true;
		public NormalsCalculation normalsCalculation = NormalsCalculation.Smooth;

		[SerializeField, HideInInspector]
		protected MeshFilter target;
		[SerializeField, HideInInspector]
		protected Chunk[] chunks;
		[SerializeField, HideInInspector]
		protected Mesh originalMesh;

		private bool usingOriginalNormals;
		private List<Vector3> originalNormals = new List<Vector3> ();

		private void OnDestroy ()
		{
			if (discardChangesOnDestroy)
				DiscardChanges ();
		}

		public void ChangeTarget (MeshFilter meshFilter)
		{
			// Assign the target.
			target = meshFilter;
			// Store the original mesh.
			originalMesh = target.sharedMesh;
			// Cache the original normals.
			target.sharedMesh.GetNormals (originalNormals);
			// Change the mesh to one we can modify.
			target.sharedMesh = MeshUtil.Copy (target.sharedMesh);

			// Create chunk data.
			RecreateChunks ();
		}

		public void RecreateChunks ()
		{
			chunkCount = Mathf.Clamp (chunkCount, 1, target.sharedMesh.vertexCount);
			chunks = ChunkUtil.CreateChunks (originalMesh, chunkCount);
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
				case NormalsCalculation.None:
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

		public void DiscardChanges ()
		{
			if (originalMesh != null && target != null)
			{
				DestroyImmediate (target.sharedMesh);
				target.sharedMesh = MeshUtil.Copy (originalMesh);
			}
		}
	}
}