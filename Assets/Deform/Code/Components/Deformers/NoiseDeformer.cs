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

		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds meshBounds)
		{
			for (int i = 0; i < meshData.Size; i++)
			{
				var sampleCoordinate = CalculateSampleCoordinate (meshData.vertices[i], transformData);
				var value = noise.GetNoise (sampleCoordinate.x, sampleCoordinate.y, sampleCoordinate.z);
				meshData.vertices[i] = TransformNoise (value, meshData.vertices[i], meshData.normals[i], meshData.tangents[i]);
			}

			return meshData;
		}

		protected override float GetFrequency ()
		{
			return frequency;
		}
	}
}