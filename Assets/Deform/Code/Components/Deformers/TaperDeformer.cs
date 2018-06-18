using UnityEngine;

namespace Deform.Deformers
{
	public class TaperDeformer : DeformerComponent
	{
		public float top = 1f;
		public float bottom = 1f;
		public AnimationCurve curve = AnimationCurve.Linear (0f, 0f, 1f, 1f);
		public Transform axis;

		private Matrix4x4 axisSpace;
		private Matrix4x4 inverseAxisSpace;

		public override void PreModify ()
		{
			base.PreModify ();

			if (axis == null)
			{
				axis = new GameObject ("TaperAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
				axis.Rotate (90f, 0f, 0f);
			}

			axisSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			inverseAxisSpace = axisSpace.inverse;
		}

		public override MeshData Modify (MeshData meshData, TransformData transformData, Bounds meshBounds)
		{
			float minHeight = float.MaxValue;
			float maxHeight = float.MinValue;

			// Find the min/max height.
			for (int i = 0; i < meshData.Size; i++)
			{
				var position = axisSpace.MultiplyPoint3x4 (meshData.vertices[i]);
				if (position.z > maxHeight)
					maxHeight = position.z;
				if (position.z < minHeight)
					minHeight = position.z;
			}

			float oneOverHeight = 1f / (maxHeight - minHeight);

			for (int i = 0; i < meshData.Size; i++)
			{
				var position = meshData.vertices[i];
				var normalizedHeight = (position.z - minHeight) * oneOverHeight;
				var scale = curve.Evaluate (normalizedHeight);
				scale *= top * (1f - normalizedHeight) + bottom * normalizedHeight;
				position.x *= scale;
				position.y *= scale;
				meshData.vertices[i] = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return meshData;
		}
	}
}