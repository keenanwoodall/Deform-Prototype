using UnityEngine;

namespace Deform.Deformers
{
	public class PivotToCenterDeformer : DeformerComponent
	{
		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				vertexData[vertexIndex].position -= meshBounds.center;
			}

			return vertexData;
		}
	}
}
