using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	public static class MeshDataUtil
	{
		public static MeshData GetMeshData (Mesh mesh)
		{
			var meshData = new MeshData ()
			{
				vertices = mesh.vertices,
				baseVertices = mesh.vertices,
				normals = mesh.normals,
				tangents = mesh.tangents,
				colors = mesh.colors
			};

			return meshData;
		}

		public static Bounds GetBounds (MeshData meshData)
		{
			var bounds = new Bounds ();
			var vertexCount = meshData.vertices.Length;
			for (int i = 0; i < vertexCount; i++)
				bounds.Encapsulate (meshData.vertices[i]);
			return bounds;
		}

		public static MeshData ResetVertexData (MeshData meshData)
		{
			var vertexCount = meshData.vertices.Length;
			for (int i = 0; i < vertexCount; i++)
				meshData.vertices[i] = meshData.baseVertices[i];

			return meshData;
		}
	}
}