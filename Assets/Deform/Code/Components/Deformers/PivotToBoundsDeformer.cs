using UnityEngine;

namespace Deform.Deformers
{
	public class PivotToBoundsDeformer : DeformerComponent
	{
		[Range (0f, 1f)]
		public float x = 0.5f, y = 0.5f, z = 0.5f;
		private Vector3 offset;

		public override void PreModify ()
		{
			var bounds = Manager.Bounds;
			offset = new Vector3 (
				bounds.min.x * (1f - x) + bounds.max.x * x,
				bounds.min.y * (1f - y) + bounds.max.y * y,
				bounds.min.z * (1f - z) + bounds.max.z * z
			);
		}
		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds meshBounds)
		{
			for (int i = 0; i < meshData.Size; i++)
				meshData.vertices[i] -= offset;

			return meshData;
		}
	}
}