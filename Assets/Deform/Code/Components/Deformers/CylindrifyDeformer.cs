using UnityEngine;

namespace Deform.Deformers
{
	public class CylindrifyDeformer : DeformerComponent
	{
		public float strength = 0f;
		public float radius = 1f;
		public Transform axis;

		private Matrix4x4 axisSpace;
		private Matrix4x4 inverseAxisSpace;

		public override void PreModify ()
		{
			base.PreModify ();

			if (axis == null)
			{
				axis = new GameObject ("CylindrifyAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
				axis.Rotate (-90f, 0f, 0f);
			}

			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			inverseAxisSpace = axisSpace.inverse;
		}

		public override VertexData[] Modify (VertexData[] vertexData, TransformData transformData, Bounds meshBounds)
		{
			float maxWidth = 0f;
			// Find the max width.
			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (vertexData[vertexIndex].position);
				var width = new Vector2 (position.x, position.y).magnitude;
				if (width > maxWidth)
					maxWidth = width;
			}

			for (int vertexIndex = 0; vertexIndex < vertexData.Length; vertexIndex++)
			{
				var position = axisSpace.MultiplyPoint3x4 (vertexData[vertexIndex].position);

				var xy = new Vector2 (position.x, position.y).normalized * radius * maxWidth;
				var goalPosition = new Vector3 (xy.x, xy.y, position.z);
				position = position * (1f - strength) + goalPosition * strength;

				vertexData[vertexIndex].position = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return vertexData;
		}
	}
}