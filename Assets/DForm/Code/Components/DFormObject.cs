using UnityEngine;

namespace DForm
{
	public class DFormObject : MonoBehaviour
	{
		private MeshFilter target;
		private Chunk[] chunks;

		private void Awake ()
		{
			target = GetComponent<MeshFilter> ();
			ChangeTarget (target);
		}

		private void Update ()
		{
			ChunkUtil.ApplyChunks (chunks, target.mesh);
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