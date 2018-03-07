using UnityEngine;

namespace Deform.Deformers
{
	public class SpherifyDeformer : DeformerComponent
	{
		public float radius = 1f;
		public float strength = 0f;
		public Vector3 offset;

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			var boundsSize = bounds.size.sqrMagnitude;
			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				chunk.vertexData[vertexIndex].position = Vector3.LerpUnclamped (
					chunk.vertexData[vertexIndex].position,
					bounds.center + offset + ((chunk.vertexData[vertexIndex].position - bounds.center).normalized * radius * boundsSize),
					strength);
			}

			return chunk;
		}
	}
}