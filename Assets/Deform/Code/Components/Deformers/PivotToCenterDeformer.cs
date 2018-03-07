using UnityEngine;

namespace Deform.Deformers
{
	public class PivotToCenterDeformer : DeformerComponent
	{
		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds meshBounds)
		{
			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				chunk.vertexData[vertexIndex].position -= meshBounds.center;
			}

			return chunk;
		}
	}
}
