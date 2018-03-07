using UnityEngine;

namespace Deform.Deformers
{
	public class BendDeformer : DeformerComponent
	{
		public float angle;
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
				axis.Rotate (-0f, 180f, 180f);
			}

			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			inverseAxisSpace = axisSpace.inverse;
		}

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			float minHeight = float.MaxValue;
			float maxHeight = float.MinValue;

			// Find the min/max height.
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (vertexData[vertexIndex].position);
				if (position.z > maxHeight)
					maxHeight = position.z;
				if (position.z < minHeight)
					minHeight = position.z;
			}

			float oneOverHeight = 1f / (maxHeight - minHeight);

			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (vertexData[vertexIndex].position);
				var normalizedHeight = (position.sqrMagnitude - minHeight) * oneOverHeight;
				var amount = angle * normalizedHeight;
				var rotation = Quaternion.AngleAxis (amount, Vector3.forward);
				position = rotation * position;

				vertexData[vertexIndex].position = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return vertexData;
		}
	}
}