using UnityEngine;

namespace Deform
{
	[System.Serializable]
	public struct Chunk
	{
		public VertexData[] vertexData;
		/// <summary>
		/// Same as vertexData.Length, just make loops a little quicker to write.
		/// </summary>
		public int Size { get { return vertexData.Length; } }

		public Chunk (VertexData[] vertexData)
		{
			this.vertexData = vertexData;
		}

		public Chunk (Vector3[] positions, Vector3[] normals, Vector4[] tangents, Color[] colors)
		{
			vertexData = new VertexData[positions.Length];

			if (tangents == null || tangents.Length == 0)
				tangents = new Vector4[vertexData.Length];
			if (colors == null || colors.Length == 0)
				colors = new Color[vertexData.Length];

			for (var i = 0; i < positions.Length; i++)
			{
				var position = positions[i];
				var normal = normals[i];
				var tangent = tangents[i];
				var color = colors[i];
				vertexData[i] = new VertexData (position, position, normal, tangent, color);
			}
		}

		public Chunk (Vector3[] basePositions, Vector3[] positions, Vector3[] normals, Vector4[] tangents, Color[] colors)
		{
			vertexData = new VertexData[positions.Length];

			if (tangents == null || tangents.Length == 0)
				tangents = new Vector4[vertexData.Length];
			if (colors == null || colors.Length == 0)
				colors = new Color[vertexData.Length];

			for (var i = 0; i < positions.Length; i++)
			{
				var basePosition = basePositions[i];
				var position = positions[i];
				var normal = normals[i];
				var tangent = tangents[i];
				var color = colors[i];
				vertexData[i] = new VertexData (basePosition, position, normal, tangent, color);
			}
		}

		public void ResetPositions ()
		{
			for (int i = 0; i < vertexData.Length; i++)
				vertexData[i].ResetPosition ();
		}
	}
}