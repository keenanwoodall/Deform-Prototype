using UnityEngine;

namespace Deform.Deformers
{
	public class SpherifyDeformer : DeformerComponent
	{
		public float radius = 1f;
		[Range (0f, 1f)]
		public float strength = 0f;
		public Vector3 offset;

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			var boundsSize = meshBounds.size.magnitude * 0.5f;
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				vertexData[vertexIndex].position = Vector3.Lerp (
					vertexData[vertexIndex].position,
					meshBounds.center + offset + ((vertexData[vertexIndex].position - meshBounds.center).normalized * (radius * boundsSize)),
					strength);
			}

			return vertexData;
		}
	}
}