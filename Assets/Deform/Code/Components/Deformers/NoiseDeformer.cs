using UnityEngine;

namespace Deform.Deformers
{
	public class NoiseDeformer : NoiseDeformerComponent
	{
		public float frequency = 1f;
		public FastNoise.NoiseType noiseType;
		public FastNoise.Interp interp;

		private FastNoise noise = new FastNoise ();

		public override void PreModify ()
		{
			base.PreModify ();
			noise.SetNoiseType (noiseType);
			noise.SetFrequency (frequency);
			noise.SetInterp (interp);
		}

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var sampleCoordinate = CalculateSampleCoordinate (vertexData[vertexIndex], transformData);
				var value = 0.5f + noise.GetNoise (sampleCoordinate.x, sampleCoordinate.y, sampleCoordinate.z);
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