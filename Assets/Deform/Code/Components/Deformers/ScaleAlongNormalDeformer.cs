using UnityEngine;

namespace Deform.Deformers
{
	public class ScaleAlongNormalDeformer : DeformerComponent
	{
		public float amount = 0f;

		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds meshBounds)
		{
			for (int i = 0; i < meshData.Size; i++)
				meshData.vertices[i] += meshData.normals[i] * amount;

			return meshData;
		}
	}
}