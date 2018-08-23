using UnityEngine;

namespace Deform.Data
{
	public class ManagedMeshData
	{
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector4[] tangents;
		public Vector2[] uv;
		public Color[] colors;

		public readonly int size;

		public ManagedMeshData (Mesh mesh)
		{
			vertices = mesh.vertices;
			normals = mesh.normals;
			tangents = mesh.tangents;
			uv = mesh.uv;
			colors = mesh.colors;
			if (colors == null)
				colors = new Color[mesh.vertexCount];

			size = vertices.Length;
		}
	}
}