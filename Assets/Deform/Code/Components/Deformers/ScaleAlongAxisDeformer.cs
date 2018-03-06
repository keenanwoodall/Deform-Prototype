using UnityEngine;

namespace Deform.Deformers
{
	public class ScaleAlongAxisDeformer : DeformerComponent
	{
		public AnimationCurve curve = AnimationCurve.Linear (0f, 0f, 1f, 1f);
		public Transform axis;

		private Matrix4x4 scaleSpace;
		private Matrix4x4 meshSpace;

		public override void PreModify ()
		{
			base.PreModify ();

			if (axis == null)
			{
				axis = new GameObject ("ScaleAlongAxisDeformerAxis").transform;
				axis.SetParent (transform);
				axis.localPosition = Vector3.zero;
				axis.Rotate (-90f, 0f, 0f);
			}

			scaleSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			meshSpace = scaleSpace.inverse;
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			float minHeight = 0f;
			float maxHeight = 0f;
			float height = 0f;

			// Find the min/max height.
			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = scaleSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				if (position.z > maxHeight)
					maxHeight = position.z;
				if (position.z < minHeight)
					minHeight = position.z;
			}
			height = maxHeight - minHeight;

			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = scaleSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				var normalizedHeight = (position.z - minHeight) / (height);
				var scale = curve.Evaluate (normalizedHeight);
				position.x *= scale;
				position.y *= scale;
				chunk.vertexData[vertexIndex].position = meshSpace.MultiplyPoint3x4 (position);
			}

			return chunk;
		}
	}
}