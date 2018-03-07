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

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			var boundsSize = bounds.size.sqrMagnitude;
			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				var rotation = Quaternion.AngleAxis (angle * (position.sqrMagnitude / boundsSize), Vector3.forward);
				position = rotation * position;

				chunk.vertexData[vertexIndex].position = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return chunk;
		}
	}
}