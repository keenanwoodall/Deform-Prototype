using UnityEngine;

namespace Deform.Data
{
	public class MeshData
	{
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector4[] tangents;
		public Vector2[] uv;

		public readonly int size;

		public MeshData (Mesh mesh)
		{
			vertices = mesh.vertices;
			normals = mesh.normals;
			tangents = mesh.tangents;
			uv = mesh.uv;

			size = vertices.Length;
		}

		public void CopyTo (MeshData other)
		{
			vertices.CopyTo (other.vertices, 0);
			normals.CopyTo (other.normals, 0);
			tangents.CopyTo (other.tangents, 0);
			uv.CopyTo (other.uv, 0);
		}
	}
}