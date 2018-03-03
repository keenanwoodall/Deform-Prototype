using UnityEngine;

namespace Deform.Deformers
{
	public class SpherizeDeformer : DeformerComponent
	{
		public float radius = 1f;
		public float strength = 0f;

#if UNITY_EDITOR
		private void OnDrawGizmosSelected ()
		{
			Gizmos.DrawWireSphere (transform.position, radius);
		}
#endif

		public override Chunk Modify (Chunk chunk)
		{
			for (var vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				chunk.vertexData[vertexIndex].position = Vector3.LerpUnclamped (
					chunk.vertexData[vertexIndex].position,
					chunk.vertexData[vertexIndex].position.normalized * radius,
					strength);
			}

			return chunk;
		}
	}
}