using UnityEngine;

namespace Deform.Deformers
{
	public class CurveDeformer : DeformerComponent
	{
		public float strength = 1f;
		public AnimationCurve curve = new AnimationCurve (new Keyframe (0f, 1f), new Keyframe (0.5f, 0.5f), new Keyframe (1f, 1f));
		public Transform axis;

		private Matrix4x4
			axisSpace,
			inverseAxisSpace;

		public override void PreModify ()
		{
			base.PreModify ();

			// Ensure there's an axis.
			if (axis == null)
			{
				axis = new GameObject ("CurveAxis").transform;
				axis.transform.SetParent (transform);
				axis.transform.Rotate (0f, 90f, 0f);
				axis.localScale = Vector3.one;
				axis.transform.localPosition = Vector3.zero;
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
				var position = axisSpace.MultiplyPoint3x4 (meshData.vertices[i]);

				var normalizedHeight = (position.z - minHeight) * oneOverHeight;

				position.y += curve.Evaluate (normalizedHeight) * strength;

				meshData.vertices[i] = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return meshData;
		}
	}
}