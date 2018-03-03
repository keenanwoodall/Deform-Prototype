using UnityEngine;

namespace Deform.Deformers
{
	public class CenterPivotDeformer : DeformerComponent
	{
		public override Chunk Modify (Chunk chunk)
		{
			for (var vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				chunk.vertexData[vertexIndex].position -= chunk.bounds.center;
			}

			return chunk;
		}
	}
}
