using UnityEngine;

namespace DForm
{
	public struct VertexData
	{
		public readonly Vector3 basePosition;
		public readonly Vector3 position;
		public readonly Vector3 normal;

		public VertexData (Vector3 basePosition, Vector3 position, Vector3 normal)
		{
			this.basePosition = basePosition;
			this.position = position;
			this.normal = normal;
		}
	}
}