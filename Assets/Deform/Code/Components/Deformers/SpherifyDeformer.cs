using UnityEngine;

namespace Deform.Deformers
{
	public class SpherifyDeformer : DeformerComponent
	{
		public float radius = 1f;
		[Range (0f, 1f)]
		public float strength = 0f;
		public Vector3 offset;

		private float oneMinusStrength;

		public override void PreModify ()
		{
			base.PreModify ();

			oneMinusStrength = 1f - strength;
		}

		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds meshBounds)
		{
			var boundsSize = meshBounds.size.magnitude * 0.5f;
			for (int i = 0; i < meshData.Size; i++)
			{
				var a = meshData.vertices[i];
				var b = meshBounds.center + offset + ((a - meshBounds.center).normalized * (radius * boundsSize));
				meshData.vertices[i] = a * oneMinusStrength + b * strength;
			}

			return meshData;
		}
	}
}