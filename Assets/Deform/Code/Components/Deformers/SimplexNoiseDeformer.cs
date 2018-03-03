using UnityEngine;
using Catlike;

namespace Deform.Deformers
{
	public class SimplexNoiseDeformer : NoiseDeformerComponent
	{
		public float frequency = 1f;

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			for (var vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var noise = Noise.SimplexValue3D (CalculateSampleCoordinate (chunk.vertexData[vertexIndex], transformData), frequency).derivative;
				chunk.vertexData[vertexIndex].position = TransformNoise (noise, chunk.vertexData[vertexIndex]);
			}

			return chunk;
		}
	}
}