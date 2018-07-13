using UnityEngine;

namespace Deform.Data
{
	public class MeshData
	{
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector4[] tangents;
		public Vector2[] uv;

		public MeshData (Mesh mesh)
		{
			vertices = mesh.vertices;
			normals = mesh.normals;
			tangents = mesh.tangents;
			uv = mesh.uv;
		}
	}
}