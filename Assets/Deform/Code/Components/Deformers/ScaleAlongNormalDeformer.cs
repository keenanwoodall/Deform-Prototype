using UnityEngine;

namespace Deform.Deformers
{
	public class ScaleAlongNormalDeformer : DeformerComponent
	{
		public float amount = 0f;

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
				vertexData[vertexIndex].position += vertexData[vertexIndex].normal * amount;

			return vertexData;
		}
	}
}