using UnityEngine;

namespace Deform.Deformers
{
	public class SquashAndStretchDeformer : DeformerComponent
	{
		[Tooltip ("You can also scale the axis' transform on the z axis to get the same effect.")]
		public float amount = 0f;
		public float offset;
		[Range (0f, 1f)]
		public float curvature = 0.75f;
		public Transform axis;

		private Matrix4x4
			axisSpace,
			inverseAxisSpace;

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
				axis.localScale = Vector3.one;
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

			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			inverseAxisSpace = axisSpace.inverse;
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			if (finalAmount == 0f)
				return chunk;

			float minHeight = float.MaxValue;
			float maxHeight = float.MinValue;

			// Find the min/max height.
			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				if (position.z > maxHeight)
					maxHeight = position.z;
				if (position.z < minHeight)
					minHeight = position.z;
			}

			float oneOverHeight = 1f / (maxHeight - minHeight);

			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);

				var normalizedHeight = (position.z - minHeight) * oneOverHeight;

				var a = curvatureMult * oneMinusOneOverFinalAmount;
				var b = -curvatureMult * oneMinusOneOverFinalAmount;
				var finalCurvature = (((a * (normalizedHeight)) + b) * (normalizedHeight)) + 1f;

				position.x *= oneOverFinalAmount * finalCurvature;
				position.y *= oneOverFinalAmount * finalCurvature;
				position.z *= finalAmount;

				chunk.vertexData[vertexIndex].position = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return chunk;
		}
	}
}