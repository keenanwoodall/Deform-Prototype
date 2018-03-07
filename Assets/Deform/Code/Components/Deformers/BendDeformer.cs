using UnityEngine;

namespace Deform.Deformers
{
	public class BendDeformer : DeformerComponent
	{
		public float angle;
		public float multiplier = 1f;
		public Transform axis;

		private Matrix4x4 axisSpace;
		private Matrix4x4 inverseAxisSpace;

		public override void PreModify ()
		{
			base.PreModify ();

			if (axis == null)
			{
				axis = new GameObject ("BendAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
				axis.Rotate (-90f, 0f, 0f);
			}

			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			inverseAxisSpace = axisSpace.inverse;
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds meshBounds)
		{
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

			float height = maxHeight - minHeight;

			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				var amount = angle * Mathf.InverseLerp (0f, height, position.sqrMagnitude);
				var rotation = Quaternion.AngleAxis (amount, Vector3.forward);
				position = rotation * position;

				chunk.vertexData[vertexIndex].position = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return chunk;
		}
	}
}