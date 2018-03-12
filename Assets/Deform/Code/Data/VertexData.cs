using UnityEngine;

namespace Deform
{
	/// <summary>
	/// Holds all information about a vertex.
	/// </summary>
	[System.Serializable]
	public struct VertexData
	{
		// Can't just have basePosition { get; private set; } because it wouldn't be serialized. :(
		[SerializeField, HideInInspector]
		private Vector3 _basePosition;
		public Vector3 basePosition { get { return _basePosition; } }
		public Vector3 position;
		public Vector3 normal;
		public Vector4 tangent;
		public Vector2 uv;
		public Color color;

		public VertexData (Vector3 basePosition, Vector3 position, Vector3 normal, Vector4 tangent, Vector2 uv, Color color)
		{
			_basePosition = basePosition;
			this.position = position;
			this.normal = normal;
			this.tangent = tangent;
			this.uv = uv;
			this.color = color;
		}

		public VertexData (Vector3 position, Vector3 normal, Vector4 tangent, Vector2 uv, Color color)
		{
			_basePosition = position;
			this.position = position;
			this.normal = normal;
			this.tangent = tangent;
			this.uv = uv;
			this.color = color;
		}

		/// <summary>
		/// Sets the position back to the basePosition.
		/// </summary>
		public void ResetPosition ()
		{
			position = basePosition;
		}
	}
}