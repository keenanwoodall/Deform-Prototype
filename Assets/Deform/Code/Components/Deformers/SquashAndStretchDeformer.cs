using UnityEngine;
namespace Deform.Deformers
{
	public class SquashAndStretchDeformer : DeformerComponent
	{
		public float amount = 0f;
		public Transform axis;

		private Matrix4x4
			scaleSpace,
			meshSpace;

		private Vector3
			scaleAxisX,
			scaleAxisY,
			scaleAxisZ;
		private TransformData cachedAxis;

		public override void PreModify ()
		{
			base.PreModify ();

			EnsureAxis ();
			cachedAxis = new TransformData (axis);

			scaleAxisZ = transform.worldToLocalMatrix.MultiplyPoint3x4 (axis.up);

			Vector3.OrthoNormalize (ref scaleAxisX, ref scaleAxisY, ref scaleAxisZ);

			scaleSpace.SetRow (0, scaleAxisX);
			scaleSpace.SetRow (1, scaleAxisY);
			scaleSpace.SetRow (2, scaleAxisZ);
			scaleSpace[3, 3] = 1f;

			scaleSpace *= Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (cachedAxis.rotation), Vector3.one);

			meshSpace = scaleSpace.inverse;
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			for (var vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = chunk.vertexData[vertexIndex].position;

				if (Mathf.Approximately (amount, 0f))
					continue;

				var basePositionOnAxis = scaleSpace.MultiplyPoint3x4 (position);
				var amountPlus1 = (amount + 1f) * cachedAxis.localScale.y;

				basePositionOnAxis.x *= 1f / amountPlus1;
				basePositionOnAxis.y *= 1f / amountPlus1;
				basePositionOnAxis.z *= amountPlus1;

				chunk.vertexData[vertexIndex].position = meshSpace.MultiplyPoint3x4 (basePositionOnAxis);
			}

			return chunk;
		}

		private void EnsureAxis ()
		{
			if (axis == null)
			{
				axis = new GameObject ("Axis").transform;
				axis.SetParent (transform);
			}
		}
	}
}