using UnityEngine;

namespace Deform
{
	public static class MeshUtil
	{
		public static Mesh Copy (Mesh mesh)
		{
			var copy = new Mesh ();
			copy.vertices = mesh.vertices;
			copy.normals = mesh.normals;
			copy.tangents = mesh.tangents;
			copy.triangles = mesh.triangles;
			copy.uv = mesh.uv;
			copy.uv2 = mesh.uv2;
			copy.uv3 = mesh.uv3;
			copy.uv4 = mesh.uv4;
			copy.colors = mesh.colors;

			return copy;
		}
	}
}