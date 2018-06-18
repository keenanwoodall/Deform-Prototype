using UnityEngine;

namespace Deform.Deformers
{
	public class PivotToCenterDeformer : DeformerComponent
	{
		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds meshBounds)
		{
			for (int i = 0; i < meshData.Size; i++)
			{
				meshData.vertices[i] -= meshBounds.center;
			}

			return meshData;
		}
	}
}
