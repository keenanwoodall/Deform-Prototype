using UnityEngine;

namespace Deform.Deformers
{
	public class SimplexNoiseDeformer : NoiseDeformerComponent
	{
		public float frequency = 1f;

		private SimplexNoise noise = new SimplexNoise ();

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds vertexDataBounds)
		{
			for (var vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var vertex = vertexData[vertexIndex];
				var samplePosition = CalculateSampleCoordinate (vertex, transformData) * frequency;
				vertex.position = TransformNoise ((float)noise.Evaluate ((float)samplePosition.x, (float)samplePosition.y, (float)samplePosition.z), vertex);
				vertexData[vertexIndex] = vertex;
			}
			return vertexData;
		}

		protected override float GetFrequency ()
		{
			return frequency;
		}
	}
}