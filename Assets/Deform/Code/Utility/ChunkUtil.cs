using System.Collections.Generic;
using UnityEngine;

namespace Deform
{
	public static class ChunkUtil
	{
		private static List<Vector3> VerticeCache = new List<Vector3> ();
		public static void ApplyChunks (Chunk[] chunks, Mesh mesh)
		{
			if (mesh == null)
			{
				Debug.Log ("Mesh is null");
				return;
			}

			mesh.GetVertices (VerticeCache);

			var vertexIndex = 0;
			for (var currentChunk = 0; currentChunk < chunks.Length; currentChunk++)
			{
				for (var currentChunkVertex = 0; currentChunkVertex < chunks[currentChunk].Size; currentChunkVertex++)
				{
					VerticeCache[vertexIndex] = chunks[currentChunk].vertexData[currentChunkVertex].position;
					vertexIndex++;
				}
			}

			mesh.SetVertices (VerticeCache);
		}

		public static Chunk CreateChunk (Mesh mesh)
		{
			return new Chunk (mesh.vertices, mesh.normals, mesh.tangents, mesh.colors);
		}

		public static Chunk[] CreateChunks (Mesh mesh, int count)
		{
			return CreateChunks (VertexDataUtil.GetVertexData (mesh), count, mesh.bounds);
		}

		public static Chunk[] CreateChunks (VertexData[] vertexData, int count, Bounds bounds)
		{
			var vertexCount = vertexData.Length;

			if (count > vertexCount)
			{
				Debug.LogWarning ("Chunk count is greater than vertex count. Setting chunk count to vertex count.");
				count = vertexCount;
			}
			else if (count < 1)
			{
				Debug.LogWarning ("Chunk count cannot be less than 1. Setting chunk count to 1.");
				count = 1;
			}

			var chunkSize = vertexCount / count;

			// Cache the mesh data.
			var vertices = VertexDataUtil.GetPositions (vertexData);
			var normals = VertexDataUtil.GetNormals (vertexData);
			var tangents = VertexDataUtil.GetTangents (vertexData);
			var colors = VertexDataUtil.GetColors (vertexData);

			// Create the array of chunks.
			var chunks = new Chunk[count];

			// Loop through each chunk.
			for (var chunkIndex = 0; chunkIndex < count; chunkIndex++)
			{
				// Calculate the start and end index of the chunk
				var startIndex = chunkSize * chunkIndex;
				var endIndex = 0;
				if (chunkIndex + 1 == count)
					endIndex = vertexData.Length;
				else
					endIndex = chunkSize * (chunkIndex + 1);

				// Create the arrays to hold the chunk data.
				var currentChunkSize = endIndex - startIndex;
				var chunkPositions = new Vector3[currentChunkSize];
				var chunkNormals = new Vector3[currentChunkSize];
				var chunkTangents = new Vector4[currentChunkSize];
				var chunkColors = new Color[currentChunkSize];

				// Loop through each vertex in the chunk.
				var chunkVertexIndex = 0;
				for (var vertexIndex = startIndex; vertexIndex < endIndex; vertexIndex++)
				{
					// Put the current vertex data into the chunk arrays.
					chunkPositions[chunkVertexIndex] = vertices[vertexIndex];
					chunkNormals[chunkVertexIndex] = normals[vertexIndex];
					chunkTangents[chunkVertexIndex] = tangents[vertexIndex];
					chunkColors[chunkVertexIndex] = colors[vertexIndex];
					chunkVertexIndex++;
				}

				// Create a chunk from the chunk arrays.
				chunks[chunkIndex] = new Chunk (chunkPositions, chunkNormals, chunkTangents, chunkColors);
			}

			return chunks;
		}

		public static Bounds GetBounds (Chunk[] chunks)
		{
			var chunksBounds = new Bounds ();
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
			{
				var chunkBounds = VertexDataUtil.GetBounds (chunks[chunkIndex].vertexData);
				chunksBounds.Encapsulate (chunkBounds);
			}

			return chunksBounds;
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

		public static Vector3[] GetTangents (Chunk[] chunks)
		{
			var tangents = new Vector3[GetChunksSize (chunks)];
			var vertexIndex = 0;
			for (var chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
			{
				for (var chunkVertexIndex = 0; chunkVertexIndex < chunks[chunkIndex].Size; chunkIndex++)
				{
					tangents[vertexIndex] = chunks[chunkIndex].vertexData[chunkVertexIndex].tangent;
					vertexIndex++;
				}
			}
			return tangents;
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