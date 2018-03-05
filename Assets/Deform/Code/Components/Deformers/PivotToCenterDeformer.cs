using UnityEngine;

namespace Deform.Deformers
{
	public class PivotToCenterDeformer : DeformerComponent
	{
		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			for (var vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				chunk.vertexData[vertexIndex].position -= bounds.center;
			}

			return chunk;
		}
	}
}
