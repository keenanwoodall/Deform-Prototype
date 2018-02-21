using UnityEngine;

namespace DForm
{
	[System.Serializable]
	public struct Chunk
	{
		public VertexData[] vertexData;
		public readonly int Size;

		public Chunk (VertexData[] vertexData)
		{
			this.vertexData = vertexData;
			this.Size = vertexData.Length;
		}

		public Chunk (Vector3[] positions, Vector3[] normals)
		{
			Size = positions.Length;
			vertexData = new VertexData[Size];

			for (var i = 0; i < positions.Length; i++)
			{
				var position = positions[i];
				var normal = normals[i];
				vertexData[i] = new VertexData (position, position, normal);
			}
		}

		public Chunk (Vector3[] basePositions, Vector3[] positions, Vector3[] normals)
		{
			Size = positions.Length;
			vertexData = new VertexData[Size];

			for (var i = 0; i < positions.Length; i++)
			{
				var basePosition = basePositions[i];
				var position = positions[i];
				var normal = normals[i];
				vertexData[i] = new VertexData (basePosition, position, normal);
			}
		}

		public void ResetPositions ()
		{
			for (int i = 0; i < vertexData.Length; i++)
				vertexData[i].ResetPosition ();
		}
	}
}