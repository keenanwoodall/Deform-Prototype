using UnityEngine;

namespace DForm
{
	public static class VertexDataUtil
	{
		public static void ApplyVertexData (Mesh mesh, VertexData[] vertexData)
		{
			var vertices = mesh.vertices;
			var vertexCount = vertices.Length;

			for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
				vertices[vertexIndex] = vertexData[vertexIndex].position;

			mesh.vertices = vertices;
		}

		public static VertexData[] GetVertexData (Mesh mesh)
		{
			var vertexCount = mesh.vertexCount;
			var vertexData = new VertexData[vertexCount];

			var vertices = mesh.vertices;
			var normals = mesh.normals;

			for (var vertexIndex = 0; vertexIndex < vertexCount; vertexIndex++)
				vertexData[vertexIndex] = new VertexData (vertices[vertexIndex], normals[vertexIndex]);

			return vertexData;
		}

		public static VertexData[] ResetVertexData (VertexData[] vertexData)
		{
			for (var vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
				vertexData[vertexIndex].ResetPosition ();

			return vertexData;
		}
	}
}