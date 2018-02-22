using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	public abstract class DeformerManagerBase : MonoBehaviour
	{
		public int chunkCount = 1;
		public bool recalculateNormals = true;
		public bool recalculateBounds = true;
		public bool discardChangesOnDestroy = true;

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

		protected void ApplyChunksToTarget ()
		{
			ChunkUtil.ApplyChunks (chunks, target.sharedMesh);

			if (recalculateNormals)
				target.sharedMesh.RecalculateNormals ();
			else
				target.sharedMesh.SetNormals (originalNormals);

			if (recalculateBounds)
				target.sharedMesh.RecalculateBounds ();
		}

		protected void ResetChunks ()
		{
			ChunkUtil.ResetChunks (chunks);
		}

		[ContextMenu ("Discard Changes")]
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