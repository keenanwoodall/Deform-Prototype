using UnityEngine;
using LibNoise.Generator;

namespace Deform.Deformers
{
	public class PerlinNoiseDeformer : NoiseDeformerComponent
	{
		public Perlin perlin = new Perlin ();

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var noise = 0.5f + (float)perlin.GetValue (CalculateSampleCoordinate (vertexData[vertexIndex], transformData));
				vertexData[vertexIndex].position = TransformNoise (noise, vertexData[vertexIndex]);
			}

			return vertexData;
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