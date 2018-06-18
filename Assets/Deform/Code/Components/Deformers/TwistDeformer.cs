using UnityEngine;

namespace Deform.Deformers
{
	public class TwistDeformer : DeformerComponent
	{
		public float angle;
		public float offset;
		public Transform axis;

		private Matrix4x4 axisSpace;
		private Matrix4x4 inverseAxisSpace;
		private TransformData axisCache;

		public override void PreModify ()
		{
			base.PreModify ();

			if (axis == null)
			{
				axis = new GameObject ("TwistAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
				axis.Rotate (-90f, 0f, 0f);
			}

			axisCache = new TransformData (axis);

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
				var amount = offset + angle * (normalizedHeight);
				position = Quaternion.Euler (0f, 0f, amount) * position;
				meshData.vertices[i] = inverseAxisSpace.MultiplyPoint3x4 (position);
			}

			return meshData;
		}
	}
}