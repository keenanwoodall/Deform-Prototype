using LibNoise.Generator;

namespace Deform.Deformers
{
	public class PerlinNoiseDeformer : NoiseDeformerComponent
	{
		public Perlin perlin = new Perlin ();

		public override Chunk Modify (Chunk chunk)
		{
			for (var vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var noise = 0.5f + (float)perlin.GetValue (CalculateSampleCoordinate (chunk.vertexData[vertexIndex], chunk.transformData));
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
	}
}