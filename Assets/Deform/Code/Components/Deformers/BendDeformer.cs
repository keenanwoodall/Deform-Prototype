using UnityEngine;

namespace Deform.Deformers
{
	public class BendDeformer : DeformerComponent
	{
		public float angle;
		public Transform axis;

		private Matrix4x4 scaleSpace;
		private Matrix4x4 meshSpace;

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

			scaleSpace = Matrix4x4.TRS (Vector3.zero, Quaternion.Inverse (axis.rotation) * transform.rotation, Vector3.one);
			meshSpace = scaleSpace.inverse;
		}

		public override Chunk Modify (Chunk chunk, TransformData transformData, Bounds bounds)
		{
			for (int vertexIndex = 0; vertexIndex < chunk.Size; vertexIndex++)
			{
				var position = scaleSpace.MultiplyPoint3x4 (chunk.vertexData[vertexIndex].position);
				var rotation = Quaternion.AngleAxis (angle * position.sqrMagnitude, Vector3.forward);
				position = rotation * position;

				chunk.vertexData[vertexIndex].position = meshSpace.MultiplyPoint3x4 (position);
			}

			return chunk;
		}
	}
}