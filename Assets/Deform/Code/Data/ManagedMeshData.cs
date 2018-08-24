using UnityEngine;

namespace Deform.Data
{
	public class ManagedMeshData
	{
		public Vector3[] vertices;
		public Vector3[] normals;
		public Vector4[] tangents;
		public Vector2[] uv;
		public int[] triangles;
		
		public readonly int size;

		public ManagedMeshData (Mesh mesh)
		{
			vertices = mesh.vertices;
			normals = mesh.normals;
			tangents = mesh.tangents;
			uv = mesh.uv;
			triangles = mesh.triangles;

			size = vertices.Length;
		}
	}
}