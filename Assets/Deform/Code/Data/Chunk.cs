using UnityEngine;

namespace Deform
{
	[System.Serializable]
	public struct Chunk
	{
		public VertexData[] vertexData;
		/// <summary>
		/// Use this instead of accessing the transform directly because your code might be run on another thread.
		/// </summary>
		public TransformData transformData;
		/// <summary>
		/// The bounds of the actual mesh, not this chunk.
		/// </summary>
		public Bounds bounds;
		/// <summary>
		/// Same as vertexData.Length, just make loops a little quicker to write.
		/// </summary>
		public int Size { get { return vertexData.Length; } }

		public Chunk (VertexData[] vertexData, TransformData transformData, Bounds bounds)
		{
			this.vertexData = vertexData;
			this.transformData = transformData;
			this.bounds = bounds;
		}

		public Chunk (Vector3[] positions, Vector3[] normals, Vector4[] tangents, Color[] colors, Bounds bounds)
		{
			vertexData = new VertexData[positions.Length];

			for (var i = 0; i < positions.Length; i++)
			{
				var position = positions[i];
				var normal = normals[i];
				var tangent = tangents[i];
				var color = colors[i];
				vertexData[i] = new VertexData (position, position, normal, tangent, color);
			}

			transformData = new TransformData ();
			this.bounds = bounds;
		}

		public Chunk (Vector3[] basePositions, Vector3[] positions, Vector3[] normals, Vector4[] tangents, Color[] colors, Bounds bounds)
		{
			vertexData = new VertexData[positions.Length];

			for (var i = 0; i < positions.Length; i++)
			{
				var basePosition = basePositions[i];
				var position = positions[i];
				var normal = normals[i];
				var tangent = tangents[i];
				var color = colors[i];
				vertexData[i] = new VertexData (basePosition, position, normal, tangent, color);
			}

			transformData = new TransformData ();
			this.bounds = bounds;
		}

		public void ResetPositions ()
		{
			for (int i = 0; i < vertexData.Length; i++)
				vertexData[i].ResetPosition ();
		}
	}
}