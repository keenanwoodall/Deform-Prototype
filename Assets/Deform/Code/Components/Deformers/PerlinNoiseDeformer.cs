using UnityEngine;
using LibNoise.Generator;

namespace Deform.Deformers
{
	public class PerlinNoiseDeformer : NoiseDeformerComponent
	{
		public Perlin perlin = new Perlin ();

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			for (var vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var noise = 0.5f + (float)perlin.GetValue (CalculateSampleCoordinate (chunk.vertexData[vertexIndex], transformData));
				chunk.vertexData[vertexIndex].position = TransformNoise (noise, chunk.vertexData[vertexIndex]);
			}

			return chunk;
		}

		public override void PostModify ()
		{
			base.PostModify ();

			if (!perlin.IsDisposed)
				perlin.Dispose ();
		}

		protected override float GetFrequency ()
		{
			return (float)perlin.Frequency;
		}
	}
}