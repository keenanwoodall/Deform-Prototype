using UnityEngine;

namespace DForm
{
	public abstract class DFormObjectBase : MonoBehaviour
	{
		protected MeshFilter target;
		protected Chunk[] chunks;

		public void ChangeTarget (MeshFilter meshFilter, Mesh mesh = null)
		{
			// Assign the target.
			target = meshFilter;
			// Change the target's mesh if new mesh is supplied.
			if (mesh == null)
				mesh = target.mesh;
			else
				target.mesh = mesh;

			// Create chunk data.
			chunks = ChunkUtil.CreateChunks (target.mesh, 1);
		}
	}
}