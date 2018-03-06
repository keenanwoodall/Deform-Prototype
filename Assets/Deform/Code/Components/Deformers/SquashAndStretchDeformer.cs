using UnityEngine;

namespace Deform.Deformers
{
	public class SquashAndStretchDeformer : DeformerComponent
	{
		[Tooltip ("You can also scale the axis' transform on the z axis to get the same effect.")]
		public float amount = 0f;
		[Range (0f, 1f)]
		public float curvature = 0.75f;
		public Transform axis;

		private Matrix4x4
			scaleSpace,
			meshSpace;

		// Calculations that don't need to be calculated for each vertex and can be cached
		private float finalAmount;
		private float oneOverFinalAmount;
		private float oneMinusOneOverFinalAmount;
		private float curvatureOverAmount;
		private float curvatureMult;

		public override void PreModify ()
		{
			base.PreModify ();

			// Ensure there's an axis.
			if (axis == null)
			{
				axis = new GameObject ("SquashAxis").transform;
				axis.transform.SetParent (transform);
				axis.transform.Rotate (-90f, 0f, 0f);
				axis.transform.localPosition = Vector3.zero;
			}

			// Calculate the amount to squash/stretch
			finalAmount = amount + axis.localScale.z;
			if (finalAmount < 0.01f)
				finalAmount = 0.01f;

			oneOverFinalAmount = 1f / finalAmount;
			oneMinusOneOverFinalAmount = 1f - oneOverFinalAmount;
			curvatureOverAmount = curvature / finalAmount;
			curvatureMult = 16f * curvatureOverAmount;

			scaleSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			meshSpace = scaleSpace.inverse;
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			if (finalAmount == 0f)
				return chunk;

			float minHeight = 0f;
			float maxHeight = 0f;
			float height = 0f;

			// Find the min/max height.
			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var positionOnAxis = scaleSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				if (positionOnAxis.z > maxHeight)
					maxHeight = positionOnAxis.z;
				if (positionOnAxis.z < minHeight)
					minHeight = positionOnAxis.z;
			}

			height = maxHeight - minHeight;

			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var positionOnAxis = scaleSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);

				var normalizedHeight = (positionOnAxis.z - minHeight) / (height);

				var a = curvatureMult * oneMinusOneOverFinalAmount;
				var b = -curvatureMult * oneMinusOneOverFinalAmount;
				var finalCurvature = (((a * (normalizedHeight)) + b) * (normalizedHeight)) + 1f;

				positionOnAxis.x *= oneOverFinalAmount * finalCurvature;
				positionOnAxis.y *= oneOverFinalAmount * finalCurvature;
				positionOnAxis.z *= finalAmount;

				chunk.vertexData[vertexIndex].position = meshSpace.MultiplyPoint3x4 (positionOnAxis);
			}

			return chunk;
		}
	}
}