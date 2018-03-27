using UnityEngine;

namespace Deform.Deformers
{
	public class NoiseDeformer : NoiseDeformerComponent
	{
		public FastNoise.NoiseType noiseType;
		public FastNoise.Interp interp;
		public float frequency = 1f;
		public int octaves = 2;
		public float lacunarity = 2f;
		public float gain = 0.5f;

		private FastNoise noise = new FastNoise ();

		public override void PreModify ()
		{
			base.PreModify ();
			noise.SetNoiseType (noiseType);
			noise.SetFrequency (frequency);
			noise.SetFractalOctaves (octaves);
			noise.SetFractalLacunarity (lacunarity);
			noise.SetFractalGain (gain);
			noise.SetInterp (interp);
		}

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var sampleCoordinate = CalculateSampleCoordinate (vertexData[vertexIndex], transformData);
				var value = noise.GetNoise (sampleCoordinate.x, sampleCoordinate.y, sampleCoordinate.z);
				vertexData[vertexIndex].position = TransformNoise (value, vertexData[vertexIndex]);
			}

			return vertexData;
		}

		protected override float GetFrequency ()
		{
			return frequency;
		}
	}
}