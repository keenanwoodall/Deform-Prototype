using UnityEngine;

namespace DForm
{
	public class DFormObject : MonoBehaviour
	{
		[SerializeField]
		private MeshFilter target;
		private Chunk[] chunks;

		private void Start ()
		{
			var chunks = CreateChunks (target.mesh, 3);
			var chunkIndex = 0;
			foreach (var chunk in chunks)
			{
				print (chunk.Count + " -----------------------------");
				var dataIndex = 0;
				foreach (var data in chunk.vertexData)
				{
					print (chunkIndex + "." + dataIndex + " - " + data.basePosition);
					dataIndex++;
				}
				chunkIndex++;
			}
		}

		private Chunk[] CreateChunks (Mesh mesh, int count)
		{
			var vertices = mesh.vertices;
			var normals = mesh.normals;

			var chunks = new Chunk[count];
			var chunkSize = mesh.vertexCount / count;
			var vertexCount = mesh.vertexCount;

			if (count > vertexCount)
			{
				Debug.LogWarning ("Chunk count is greater than vertex count.");
				count = vertexCount;
			}

			for (var chunkIndex = 0; chunkIndex < count; chunkIndex++)
			{
				var startIndex = chunkSize * chunkIndex;
				var endIndex = chunkSize * (chunkIndex + 1);
				if (chunkIndex + 1 == count)
					endIndex += vertexCount - (chunkSize * count);

				var chunkLength = endIndex - startIndex;
				var chunkPositions = new Vector3[chunkLength];
				var chunkNormals = new Vector3[chunkLength];

				var chunkVertexIndex = 0;
				for (var vertexIndex = startIndex; vertexIndex < endIndex; vertexIndex++)
				{
					chunkPositions[chunkVertexIndex] = vertices[vertexIndex];
					chunkNormals[chunkVertexIndex] = normals[vertexIndex];
					chunkVertexIndex++;
				}

				chunks[chunkIndex] = new Chunk (chunkPositions, chunkNormals);
			}

			return chunks;
		}
	}
}