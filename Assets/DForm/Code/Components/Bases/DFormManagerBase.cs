using UnityEngine;

namespace DForm
{
	public abstract class DFormManagerBase : MonoBehaviour
	{
		public bool recalculateNormals, recalculateTangents;
		public bool discardChangesOnDestroy = true;

		protected MeshFilter target;
		protected Chunk[] chunks;

		private Mesh originalMesh;

		private void OnDestroy ()
		{
			if (discardChangesOnDestroy && originalMesh != null && target != null)
				target.sharedMesh = target.mesh = originalMesh;
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
			chunks = ChunkUtil.CreateChunks (target.sharedMesh, 1);
		}

		protected void ApplyChunksToTarget ()
		{
			ChunkUtil.ApplyChunks (chunks, target.sharedMesh);
			target.sharedMesh.RecalculateBounds ();
			if (recalculateNormals)
				target.sharedMesh.RecalculateNormals ();
			else
				target.sharedMesh.normals = originalMesh.normals;
			if (recalculateTangents)
				target.sharedMesh.RecalculateTangents ();
			else
				target.sharedMesh.tangents = originalMesh.tangents;
		}

		protected void ResetChunks ()
		{
			ChunkUtil.ResetChunks (chunks);
		}
	}
}