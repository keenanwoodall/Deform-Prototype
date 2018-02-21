using UnityEngine;

namespace DForm
{
	[System.Serializable]
	public struct VertexData
	{
		public readonly Vector3 basePosition;
		public Vector3 position;
		public Vector3 normal;

		public VertexData (Vector3 basePosition, Vector3 position, Vector3 normal)
		{
			this.basePosition = basePosition;
			this.position = position;
			this.normal = normal;
		}

		public VertexData (Vector3 position, Vector3 normal)
		{
			this.basePosition = position;
			this.position = position;
			this.normal = normal;
		}

		public void ResetPosition ()
		{
			position = basePosition;
		}
	}
}