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

		public void ChangeTarget (MeshFilter meshFilter, Mesh mesh = null)
		{
			target = meshFilter;
			if (mesh == null)
				mesh = target.mesh;
			else
				target.mesh = mesh;

			chunks = CreateChunks (target.mesh, 1);
		}

		private Chunk[] CreateChunks (Mesh mesh, int count)
		{
			// Cache the mesh data.
			var vertices = mesh.vertices;
			var normals = mesh.normals;

			// Create the array of chunks.
			var chunks = new Chunk[count];
			// Cache chunk info.
			var chunkSize = mesh.vertexCount / count;
			var vertexCount = mesh.vertexCount;

			if (count > vertexCount)
			{
				Debug.LogWarning ("Chunk count is greater than vertex count.");
				count = vertexCount;
			}

			// Loop through each chunk.
			for (var chunkIndex = 0; chunkIndex < count; chunkIndex++)
			{
				// Calculate the start and end index of the chunk
				var startIndex = chunkSize * chunkIndex;
				var endIndex = chunkSize * (chunkIndex + 1);
				if (chunkIndex + 1 == count)
					endIndex += vertexCount - (chunkSize * count);

				// Create the arrays to hold the chunk data.
				var chunkLength = endIndex - startIndex;
				var chunkPositions = new Vector3[chunkLength];
				var chunkNormals = new Vector3[chunkLength];

				// Loop through each vertex in the chunk.
				var chunkVertexIndex = 0;
				for (var vertexIndex = startIndex; vertexIndex < endIndex; vertexIndex++)
				{
					// Put the current vertex data into the chunk arrays.
					chunkPositions[chunkVertexIndex] = vertices[vertexIndex];
					chunkNormals[chunkVertexIndex] = normals[vertexIndex];
					chunkVertexIndex++;
				}

				// Create a chunk from the chunk arrays.
				chunks[chunkIndex] = new Chunk (chunkPositions, chunkNormals);
			}

			return chunks;
		}

		private Chunk[] ResetChunksPositions (Chunk[] chunks)
		{
			for (int i = 0; i < chunks.Length; i++)
				chunks[i].ResetPositions ();
			return chunks;
		}

		private void PrintChunks (Chunk[] chunks)
		{
			var printString = "Chunk Data:\n";
			var chunkIndex = 1;
			foreach (var chunk in chunks)
			{
				printString += "Chunk Index " + chunkIndex + ", Size " + chunk.Count + "\n";
				var dataIndex = 1;
				foreach (var data in chunk.vertexData)
				{
					printString += @"    Vertex " + dataIndex + " - " + data.basePosition + "\n";
					dataIndex++;
				}
				chunkIndex++;
			}
			Debug.Log (printString);
		}
	}
}