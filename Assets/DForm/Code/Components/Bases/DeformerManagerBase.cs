using UnityEngine;

namespace Deform
{
	public abstract class DeformerManagerBase : MonoBehaviour
	{
		public int chunkCount = 2;
		public bool recalculateNormals = true;
		public bool discardChangesOnDestroy = true;

		[SerializeField, HideInInspector]
		protected MeshFilter target;
		[SerializeField, HideInInspector]
		protected Chunk[] chunks;
		[SerializeField, HideInInspector]
		protected Mesh originalMesh;

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
			// Change the mesh to one we can modify.
			target.sharedMesh = MeshUtil.Copy (target.sharedMesh);

			// Create chunk data.
		}

		public void CreateChunks (int count)
		{
			chunks = ChunkUtil.CreateChunks (target.sharedMesh, count);
		}

		protected void ApplyChunksToTarget ()
		{
			ChunkUtil.ApplyChunks (chunks, target.sharedMesh);

			if (recalculateNormals)
				target.sharedMesh.RecalculateNormals ();

			target.sharedMesh.RecalculateBounds ();
		}

		protected void ResetChunks ()
		{
			ChunkUtil.ResetChunks (chunks);
		}

		[ContextMenu ("Discard Changes")]
		public void DiscardChanges ()
		{
			recalculateNormals = false;
			if (originalMesh != null && target != null)
				target.sharedMesh = MeshUtil.Copy (originalMesh);
		}
	}
}