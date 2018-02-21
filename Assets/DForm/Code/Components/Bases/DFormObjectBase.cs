using UnityEngine;

namespace DForm
{
	public abstract class DFormObjectBase : MonoBehaviour
	{
		private MeshFilter target;
		private Chunk[] chunks;

		private void Start ()
		{
			target = GetComponent<MeshFilter> ();
			ChangeTarget (target);
		}

		private void Update ()
		{
			ChunkUtil.ApplyChunks (chunks, target.mesh);
			ChunkUtil.ResetChunks (chunks);
		}

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