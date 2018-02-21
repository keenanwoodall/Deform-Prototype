using UnityEngine;

namespace DForm
{
	public static class ChunkUtil
	{
		public static void ApplyChunks (Chunk[] chunks, Mesh mesh)
		{
			var vertices = mesh.vertices;
			var vertexIndex = 0;
			for (var currentChunk = 0; currentChunk < chunks.Length; currentChunk++)
			{
				for (var currentChunkVertex = 0; currentChunkVertex < chunks[currentChunk].Size; currentChunkVertex++)
				{
					vertices[vertexIndex] = chunks[currentChunk].vertexData[currentChunkVertex].position;
					vertexIndex++;
				}
			}

			mesh.vertices = vertices;
		}

		public static Chunk CreateChunk (Mesh mesh)
		{
			return new Chunk (mesh.vertices, mesh.normals);
		}

		public static Chunk[] CreateChunks (Mesh mesh, int count)
		{
			return CreateChunks (VertexDataUtil.GetVertexData (mesh), count);
		}

		public static Chunk[] CreateChunks (VertexData[] vertexData, int count)
		{
			// Cache the mesh data.
			var vertices = VertexDataUtil.GetPositions (vertexData);
			var normals = VertexDataUtil.GetNormals (vertexData);

			// Create the array of chunks.
			var chunks = new Chunk[count];
			// Cache chunk info.
			var vertexCount = vertexData.Length;
			var chunkSize = vertexCount / count;

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

		public static Vector3[] GetBasePositions (Chunk[] chunks)
		{
			var basePositions = new Vector3[GetChunksSize (chunks)];
			var vertexIndex = 0;
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
			{
				for (var chunkVertexIndex = 0; chunkVertexIndex < chunks[chunkIndex].Size; chunkIndex++)
				{
					basePositions[vertexIndex] = chunks[chunkIndex].vertexData[chunkVertexIndex].basePosition;
					vertexIndex++;
				}
			}
			return basePositions;
		}

		public static Vector3[] GetPositions (Chunk[] chunks)
		{
			var positions = new Vector3[GetChunksSize (chunks)];
			var vertexIndex = 0;
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
			{
				for (var chunkVertexIndex = 0; chunkVertexIndex < chunks[chunkIndex].Size; chunkIndex++)
				{
					positions[vertexIndex] = chunks[chunkIndex].vertexData[chunkVertexIndex].position;
					vertexIndex++;
				}
			}
			return positions;
		}

		public static Vector3[] GetNormals (Chunk[] chunks)
		{
			var normals = new Vector3[GetChunksSize (chunks)];
			var vertexIndex = 0;
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
			{
				for (var chunkVertexIndex = 0; chunkVertexIndex < chunks[chunkIndex].Size; chunkIndex++)
				{
					normals[vertexIndex] = chunks[chunkIndex].vertexData[chunkVertexIndex].normal;
					vertexIndex++;
				}
			}
			return normals;
		}

		public static int GetChunksSize (Chunk[] chunks)
		{
			var vertexCount = 0;
			for (int i = 0; i < chunks.Length; i++)
				vertexCount += chunks[i].Size;
			return vertexCount;
		}

		public static Chunk[] ResetChunks (Chunk[] chunks)
		{
			for (int i = 0; i < chunks.Length; i++)
				chunks[i].ResetPositions ();
			return chunks;
		}

		public static void PrintChunks (Chunk[] chunks)
		{
			var printString = "Chunk Data:\n";
			var chunkIndex = 1;
			foreach (var chunk in chunks)
			{
				printString += "Chunk Index " + chunkIndex + ", Size " + chunk.Size + "\n";
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