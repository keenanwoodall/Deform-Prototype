using UnityEngine;

namespace DForm
{
	public abstract class DFormObjectBase : MonoBehaviour
	{
		protected MeshFilter target;
		protected Chunk[] chunks;

		public void ChangeTarget (MeshFilter meshFilter, Mesh mesh = null)
		{
			target = meshFilter;
			if (mesh == null)
				mesh = target.mesh;
			else
				target.mesh = mesh;

			chunks = ChunkUtil.CreateChunks (target.mesh, 1);
		}
	}
}